function saveNewsID(newsID) {

    console.log(newsID)
    let newsIds = JSON.parse(localStorage.getItem('NewsIDs')) || [];
    var readListInput = document.getElementById('read-list-url');

    if (newsIds.includes(newsID)) {
        newsIds = newsIds.filter(id => id !== newsID);
    } else {
        newsIds.push(newsID);
    }
    localStorage.setItem('NewsIDs', JSON.stringify(newsIds));
    readListInput.value = newsIds.join(',');
    
    showNotification("Haber başarılı bir şekilde kaydedildi.");
}

function showNotification(message) {
    const notification = document.getElementById('notification');
    notification.textContent = message;
    notification.classList.add('show');
    notification.classList.remove('hidden');

    setTimeout(() => {
        notification.classList.add('hidden');
    }, 3000);
}

function calculateCurrency() {
    let amount = Number(document.getElementById("amountInput").value) || 0;
    amount = Math.floor(amount);

    let currencySlug = document.getElementById("currency-select").value;

    if (!amount || !currencySlug) {
        return;
    }

    let newUrl = `/doviz/${amount}-${currencySlug}-kac-tl`;
    window.location.href = newUrl;
}

document.addEventListener("DOMContentLoaded", function () {
    const buttons = document.querySelectorAll(".clickSubsButton");

    if (buttons.length > 0) {
        buttons.forEach(btn => {
            btn.addEventListener("click", function () {
                var emailInput = document.getElementById("setrowEmailInput");
                var groupIdInput = getSelectedOption();
                var email = emailInput != null ? emailInput.value : "";
                var groupIds = [];
                if (groupIdInput && Array.isArray(groupIdInput)) {
                    groupIds = groupIdInput
                        .map(val => parseInt(val, 10))
                        .filter(num => !isNaN(num));
                }
                CreateSetrowSubscription(email, groupIds);
            });

        });
    }
});

const aylik = document.getElementById("aylik");
const haftalik = document.getElementById("haftalik");

if (aylik && haftalik) {
    
    window.getSelectedOption = function () {
        return [aylik, haftalik].filter(item => item.checked).map(item => item.value);
    };
}



function CreateSetrowSubscription(email, groupIds) {
    var name = email.split('@')[0];

    var data = {
        Email: email,
        Name: name,
        GroupIDs: groupIds
    };

    $.ajax({
        type: "POST",
        data: data,
        url: "/Common/AddSetrowanSubscription",
        success: function (response) {
            if (response.isSuccess) {
                showNotification("Abonelik başarılı bir şekilde oluşturuldu.");
            }
        },
        error: function (error) {
            showNotification("Abonelik oluşturulurken bir hata oluştu.");
        }
    });
}
