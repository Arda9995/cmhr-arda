/**
 * Survey.js - Anket işlemleri için JavaScript kodu
 * Bu dosya anket oylaması, sonuçların gösterilmesi ve localStorage kontrolü için kullanılır
 */

document.addEventListener("DOMContentLoaded", function() {
    // Anket listesini seç
    const surveyList = document.querySelectorAll(".survey-list .item");
    
    if (surveyList.length === 0) {
        return;
    }
    
    // Daha önce oy kullanılıp kullanılmadığını kontrol et
    checkPreviousVote();
    
    // Anket öğelerine tıklama olayı ekle
    surveyList.forEach(item => {
        // Mevcut tüm event listener'ları kaldıralım
        const newItem = item.cloneNode(true);
        item.parentNode.replaceChild(newItem, item);
        
        // Yeni event listener ekleyelim
        newItem.addEventListener("click", function(event) {
            handleSurveyVote.call(this, event);
        });
    });
    
    /**
     * Ankete oy verme işlemi
     * @param {Event} event Tıklama olayı
     */
    function handleSurveyVote(event) {
        if (isVotingDisabled()) {
            return;
        }
        
        const clickedItem = this;
        
        // Doğrudan input elementlerini alalım
        const optionIdcomp = clickedItem.querySelector('#option-id');
        const surveyIdcomp = clickedItem.querySelector('#survey-id');
        
        const surveyID = surveyIdcomp.value;
        const optionID = optionIdcomp.value;
        
        // Seçilen öğeyi işaretle
        selectOption(clickedItem);
        
        // Tüm anket öğelerini devre dışı bırak
        disableAllItems();
        
        // Oy bilgisini localStorage'e kaydet
        saveVoteToLocalStorage(surveyID, optionID);
        
        // Sunucuya oy gönder
        sendVoteToServer(surveyID, optionID);
        
        // Anket sonuçlarını göster
        showSurveyResults();
    }
    
    function sendVoteToServer(surveyId, optionId) {
        const surveyIdInt = parseInt(surveyId, 10);
        const optionIdInt = parseInt(optionId, 10);
        
        $.ajax({
            url: '/News/VoteSurvey',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ 
                SurveyId: surveyIdInt, 
                OptionId: optionIdInt 
            }),
            success: function(data) {
                if (data.success) {
                    getSurveyResults();
                }
            },
            error: function(err) {
            }
        });
    }
    
    function getSurveyResults() {
        var surveyId = getSurveyId();
        if (!surveyId) {
            return;
        }
        $.ajax({
            url: '/News/GetSurveyResult/' + surveyId,
            type: 'GET',
            contentType: 'application/json',
            success: function (data) {
                if (data.success && data.data && data.data.surveyOptions) {
                    updateSurveyResultsDom(data.data.surveyOptions);
                }
            },
            error: function (err) {
                // Hata durumunda sessizce devam et
            }
        });
    }

    /**
     * Sunucudan gelen güncel sonuçlara göre UI'ı günceller
     * @param {Array} surveyOptions [{ optionId, percent, voteCount, isUserVoted }]
     */
    function updateSurveyResultsDom(surveyOptions) {
        // Her bir seçenek için güncelle
        surveyOptions.forEach(function (opt) {
            // Her item'ın içinde option-id input'u var
            var items = document.querySelectorAll('.survey-list .item');
            items.forEach(function (item) {
                var optionInput = item.querySelector('#option-id');
                if (optionInput && parseInt(optionInput.value, 10) === opt.optionID) {
                    // Progress bar'ı güncelle
                    var progressBar = item.querySelector('.progress');
                    if (progressBar) {
                        progressBar.style.width = opt.percentRatio + '%';
                    }
                    // Yüzdeyi güncelle
                    var resultElement = item.querySelector('.result');
                    if (resultElement) {
                        resultElement.textContent = opt.percentRatio + '%';
                        resultElement.classList.remove('hidden');
                    }
                    // Kullanıcı bu seçeneğe oy verdiyse işaretle
                    if (opt.isUserVoted) {
                        selectOption(item);
                    }
                }
            });
        });

        // Tüm seçenekleri devre dışı bırak
        disableAllItems();
    }

    /**
     * Anket sonuçlarını göster
     */
    function showSurveyResults() {
        surveyList.forEach((item, index) => {
            // Doğrudan input elementlerini alalım
            const allInputs = item.querySelectorAll('input[type="hidden"]');
            if (allInputs.length < 3) {
                return;
            }
            
            const percentValue = allInputs[2].value || "0";
            const progressBar = item.querySelector('.progress');
            const resultElement = item.querySelector('.result');
            
            if (progressBar) {
                progressBar.style.width = `${percentValue}%`;
            }
            
            if (resultElement) {
                resultElement.textContent = `${percentValue}%`;
                resultElement.classList.remove('hidden');
            }
        });
    }
    
    /**
     * Seçenek seçme
     * @param {HTMLElement} item Seçilen anket öğesi
     */
    function selectOption(item) {
        // Tüm öğelerden active sınıfını kaldır
        surveyList.forEach(el => {
            el.classList.remove("active");
        });
        
        // Seçilen öğeye active sınıfı ekle
        item.classList.add("active");
        
        // Seçim işaretini güncelle
        const check = item.querySelector(".check");
        if (check) {
            check.classList.remove("border-[#6B6B7D]");
            check.classList.add("border-[#FF8800]");
            
            const checked = check.querySelector(".checked");
            if (checked) {
                checked.classList.remove("opacity-0");
            }
        }
        
        // Sonuç göstergesini güncelle
        const result = item.querySelector(".result");
        if (result) {
            result.classList.remove("text-[#FF8800]");
            result.classList.add("text-white", "bg-[#FF8800]");
        }
    }
    
    /**
     * Tüm anket öğelerini devre dışı bırak
     */
    function disableAllItems() {
        surveyList.forEach((el, index) => {
            el.classList.add("pointer-events-none");
            
            const result = el.querySelector(".result");
            if (result) {
                result.classList.remove("hidden");
            }
        });
    }
    
    /**
     * Daha önce oy kullanılıp kullanılmadığını kontrol et
     */
    function checkPreviousVote() {
        const surveyId = getSurveyId();
        
        if (!surveyId) {
            return;
        }
        
        const localStorageKey = `survey_vote_${surveyId}`;
        const savedVote = localStorage.getItem(localStorageKey);
        
        if (savedVote) {
            const optionId = savedVote;
            
            // Daha önce oy verilen seçeneği bul ve seç
            let found = false;
            surveyList.forEach((item, index) => {
                // Doğrudan input elementlerini alalım
                const allInputs = item.querySelectorAll('input[type="hidden"]');
                if (allInputs.length < 2) {
                    return;
                }
                
                const itemOptionId = allInputs[1].value;
                
                if (itemOptionId == optionId) {
                    selectOption(item);
                    found = true;
                }
            });
            
            // Tüm anket öğelerini devre dışı bırak
            disableAllItems();
            
            // Anket sonuçlarını göster
            showSurveyResults();
        }
    }
    
    /**
     * Oy bilgisini localStorage'e kaydet
     * @param {string} surveyId Anket ID
     * @param {string} optionId Seçenek ID
     */
    function saveVoteToLocalStorage(surveyId, optionId) {
        const key = `survey_vote_${surveyId}`;
        
        try {
            localStorage.setItem(key, optionId);
        } catch (error) {
            // localStorage hatası durumunda sessizce devam et
        }
    }
    
    /**
     * Anket ID'sini getir
     * @returns {string|null} Anket ID
     */
    function getSurveyId() {
        const firstItem = document.querySelector(".survey-list .item");
        if (!firstItem) {
            return null;
        }
        
        const allInputs = firstItem.querySelectorAll('input[type="hidden"]');
        if (allInputs.length < 1) {
            return null;
        }
        
        return allInputs[0].value;
    }
    
    /**
     * Oylama devre dışı mı kontrol et
     * @returns {boolean} Oylama devre dışıysa true
     */
    function isVotingDisabled() {
        const isDisabled = document.querySelector(".survey-list .item.active") !== null;
        return isDisabled;
    }
});
