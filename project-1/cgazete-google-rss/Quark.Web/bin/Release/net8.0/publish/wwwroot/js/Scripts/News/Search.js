document.addEventListener("DOMContentLoaded", function () {

    let typingTimer;
    const doneTypingInterval = 300;
    const searchInput = document.getElementById('search-news-input');
    const resultList = document.getElementById('search-page-ul');
    const searchPageHref = document.getElementById('search-page-href');
    const searchForm = document.getElementById('searchPageForm');
    const searchPageResults = document.getElementById('searchPageResults');
    
    function searchNews(query) {
        fetch(`/News/LiveSearch?search=${query}`, {
            method: 'POST',
        })
            .then(response => response.json())
            .then(data => {
                resultList.innerHTML = '';
                if (data && data.length > 0) {
                    data.forEach(item => {
                        var html = `
                        <li>
                            <a href="${item.slug}" class="flex items-center gap-2">
                                <div class="aspect-[16/9] w-[118px]"> 
                                    <img src="${item.imageURL}" class="w-full h-full object-cover" alt="" loading="lazy"> 
                                </div> 
                                <div class="flex-1">
                                    <div class="line-clamp-1 font-semibold text-slate-900"> 
                                        ${truncateText(item.headline, 40)} 
                                    </div> 
                                    <p class="text-slate-900 text-sm font-medium"> 
                                        ${truncateText(item.upSubtitle, 100)} 
                                    </p> 
                                </div> 
                            </a> 
                            <span class="block bg-gradient-to-r from-transparent via-[#E5E5EA] to-transparent h-px my-4"></span>
                        </li>
                    `;
                        resultList.insertAdjacentHTML('beforeend', html);
                    });
                } else {
                    resultList.innerHTML = '<li>Sonuç bulunamadı.</li>';
                }
            })
            .catch(error => {
                resultList.innerHTML = '<li>Bir hata oluştu.</li>';
            });
    }

    searchForm.addEventListener('submit', function (e) {
        e.preventDefault();
        const searchKey = searchInput.value.trim();

        if (searchKey) {
            const encodedSearchKey = searchKey;
            window.location.href = `/arama/${encodedSearchKey}`;
        }
    });

});

function truncateText(text, maxLength) {
    if (!text) return '';
    if (text.length <= maxLength) {
        return text;
    }
    return text.substring(0, maxLength) + '...';
}


$(document).ready(function () {
    let currentPage = 1;
    let isLoading = false;
    let hasMoreItems = true;
    let currentDateRange = '';

    // Handle filter button click
    $('#filterButton').on('click', function () {
        const dateRange = $('#datepicker').val();
        currentDateRange = dateRange;
        currentPage = 1;
        loadArticles(true);
    });

    // Handle load more button click
    $('#loadMoreBtn').on('click', function () {
        if (!isLoading && hasMoreItems) {
            currentPage++;
            loadArticles(false);
        }
    });

    // Function to load articles
    function loadArticles(resetContainer) {
        if (isLoading) return;

        isLoading = true;

        $.ajax({
            url: '/News/LoadMoreSearchResults',
            type: 'GET',
            data: {
                search: $('#search-key').val(),
                dateRange: currentDateRange,
                page: currentPage,
            },
            success: function (response) {
                isLoading = false;
                hasMoreItems = response.hasMoreItems;
                totalCount = response.totalCount;
                $('#total-news-count').text("Toplam " + totalCount + " sonuç");
                if (!hasMoreItems) {
                    $('#loadMoreBtn').hide();
                } else {
                    $('#loadMoreBtn').show();
                }

                if (resetContainer) {
                    $('#search-results-container').empty();
                    $('#search-results-container-top').empty();
                }

                if (response.items && response.items.length > 0) {
                    response.items.forEach(function (item, index) {
                        const itemHtml = `
            <div class="group block relative news-item">
                <button onclick="saveNewsID(${item.id})" class="opacity-0 group-hover:opacity-100 transition-all duration-500 font-semibold text-sm z-10 absolute right-1.5 top-1.5 bg-white/70 size-7 flex items-center justify-center rounded-lg">
                    <svg width="12" height="16" viewBox="0 0 12 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="..." fill="#FF0101"></path>
                    </svg>
                </button>
                <a href="${item.slug}" class="overflow-hidden relative aspect-[16/9] mb-2 block ">
                    <img src="${item.imageURL}" class="w-full h-full object-cover animate-img" alt="${item.headline}" loading="lazy">
                    <span class="absolute text-xs text-secondary-500 font-semibold px-1 bg-white/70 backdrop-blur-sm bottom-3 left-3">
                        ${item.publishDate}
                    </span>
                </a>
                <a href="${item.slug}" class="font-semibold line-clamp-2 text-secondary-500 mb-1">${item.headline}</a>
                <a href="${item.slug}" class="text-sm group-hover:underline line-clamp-2 text-secondary-400 font-medium">${item.upSubtitle}</a>
            </div>
        `;

                        // Eğer ilk 6 haber ise farklı şablonla top'a ekle
                        if (index < 6 && resetContainer) {
                            const topItemHtml = `
                       <div class="group block relative ">
            <button onclick="saveNewsID(${item.id})" class="opacity-0 group-hover:opacity-100 transition-all duration-500 font-semibold text-sm z-10 absolute right-1.5 top-1.5 bg-white/70 size-7 flex items-center justify-center rounded-lg">
                <svg width="12" height="16" viewBox="0 0 12 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M0 2.5C0 1.83696 0.263392 1.20107 0.732233 0.732233C1.20107 0.263392 1.83696 0 2.5 0H9.5C10.163 0 10.7989 0.263392 11.2678 0.732233C11.7366 1.20107 12 1.83696 12 2.5V15.5C11.9999 15.5922 11.9744 15.6826 11.9261 15.7611C11.8779 15.8397 11.8089 15.9034 11.7267 15.9452C11.6446 15.987 11.5525 16.0053 11.4606 15.998C11.3687 15.9907 11.2806 15.9582 11.206 15.904L6 12.118L0.794 15.904C0.719435 15.9582 0.631335 15.9907 0.539436 15.998C0.447537 16.0053 0.355417 15.987 0.273255 15.9452C0.191094 15.9034 0.122089 15.8397 0.0738659 15.7611C0.0256429 15.6826 7.93406e-05 15.5922 0 15.5V2.5ZM2.5 1C2.10218 1 1.72064 1.15804 1.43934 1.43934C1.15804 1.72064 1 2.10218 1 2.5V14.518L5.706 11.096C5.79144 11.0339 5.89436 11.0004 6 11.0004C6.10564 11.0004 6.20856 11.0339 6.294 11.096L11 14.518V2.5C11 2.10218 10.842 1.72064 10.5607 1.43934C10.2794 1.15804 9.89782 1 9.5 1H2.5Z" fill="#FF0101"></path>
                </svg>
            </button><!-- Thumbnail Start -->
            <a href="${item.slug}" class="overflow-hidden relative aspect-[16/9] mb-2 block ">
                <img src="${item.imageURL}" class="w-full h-full object-cover animate-img" alt="${item.headline}" loading="lazy">
                <span class="absolute text-xs text-secondary-500 font-semibold px-1 bg-white/70 backdrop-blur-sm bottom-3 left-3">${item.publishDate}</span>
            </a><!-- Thumbnail End --><!-- Title Start -->
            <a href="${item.slug}" class="font-semibold line-clamp-2 text-secondary-500 mb-1">
                ${item.headline}
            </a><!-- Title End --><!-- Description Start -->
            <a href="${item.slug}" class="text-sm group-hover:underline line-clamp-2 text-secondary-400 font-medium">${item.upSubtitle}</a>
        </div>


            `;
                            $('#search-results-container-top').append(topItemHtml);
                        } else {
                            // Geri kalanlar normal şekilde alta eklenecek
                            $('#search-results-container').append(itemHtml);
                        }
                    });
                } else if (resetContainer) {
                    $('#articles-container').html('<div class="col-span-full text-center p-5">Bu tarih aralığında yazı bulunamadı.</div>');
                    $('#loadMoreBtn').hide();
                }

              
            },
            error: function () {
                isLoading = false;
            }
        });
    }
});