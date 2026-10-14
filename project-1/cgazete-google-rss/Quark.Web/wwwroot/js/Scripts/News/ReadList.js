document.addEventListener('DOMContentLoaded', function () {
    // Get saved news IDs from localStorage
    const savedNewsIds = JSON.parse(localStorage.getItem('NewsIDs')) || [];
    if (savedNewsIds.length > 0) {
        // Convert array to comma-separated string
        const readList = savedNewsIds.join(',');
        
        // Fetch news items from the server
        fetchReadListItems(readList, 1); // Start with page 1
    } else {
        // Optionally show a message to the user
        const postsListContainer = document.querySelector('.posts-list');
        if (postsListContainer) {
            postsListContainer.innerHTML = '<div class="text-center py-8">Henüz okuma listenizde haber bulunmamaktadır.</div>';
        }
    }
});


function fetchReadListItems(readList, page = 1) {
    fetch('/Common/GetReadListItem', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            readList: readList,
            page: page
        })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        renderReadListItems(data.items, data.hasMoreItems, page, readList);
    })
    .catch(error => {
    });
}


function renderReadListItems(items, hasMoreItems, currentPage, readList) {
    const postsListContainer = document.querySelector('.posts-list');
    
    if (!postsListContainer) {
        return;
    }
    
    if (!items || items.length === 0) {
        postsListContainer.innerHTML = '<div class="text-center py-8">Henüz okuma listenizde haber bulunmamaktadır.</div>';
        return;
    }
    
    // If this is page 1, clear the container first
    if (currentPage === 1) {
        postsListContainer.innerHTML = '';
    } else {
        // Remove the existing load more button if it exists
        const existingLoadMoreButton = postsListContainer.querySelector('.load-more-container');
        if (existingLoadMoreButton) {
            existingLoadMoreButton.remove();
        }
    }
    
    // Add the news items
    items.forEach(news => {
        const newsElement = document.createElement('div');
        newsElement.className = 'flex items-center gap-3 item';
        newsElement.innerHTML = `
            <input type="checkbox" class="hidden size-4">
            <div class="flex gap-3 flex-1">
                <!-- Thumbnail Start -->
                <a href="${news.slug}" class="block aspect-[16/9] shrink-0 w-24 md:w-[218px]">
                    <img class="w-full h-full object-cover shrink-0" src="${news.imageURL}" alt="${news.headline}" loading="lazy">
                </a>
                <!-- Thumbnail End -->
                <!-- Content Start -->
                <div class="flex-1 flex flex-col">
                    <!-- Title Start -->
                    <a href="${news.slug}" class="line-clamp-1 md:line-clamp-2 font-semibold text-sm md:text-xl">${news.headline}</a>
                    <!-- Title End -->
                    <!-- Description Start -->
                    <a href="${news.slug}" class="mt-2 mb-3 line-clamp-1 font-helvetica hidden md:block">${news.upSubtitle}</a>
                    <!-- Description End -->
                    <!-- Meta Start -->
                    <div class="flex gap-1 text-xs text-[#6B6B7D] items-center font-helvetica mt-auto">
                        <div class="block">Kaydedildi:${news.saveDate}</div>
                        <div class="w-px h-2 bg-[#9c9ca9] hidden lg:block"></div>
                        <div class="block">Yayınlandı:${news.publishDate}</div>
                        <div class="ml-auto flex items-center gap-2">
                            <button class="text-xs bg-red-50 text-red-400 hover:bg-red-100 hover:text-red-500 transition-all py-1 px-3 rounded-sm" 
                                    onclick="removeFromReadList(${news.id})">Sil</button>
                            <!-- Social Start -->
                            <div class="hidden share-link-container">
                                <button>
                                    <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <path d="M10.0017 4.00001C10.0015 3.53136 10.1659 3.07754 10.4662 2.71774C10.7665 2.35794 11.1836 2.11502 11.6447 2.03137C12.1058 1.94772 12.5817 2.02866 12.9892 2.26007C13.3967 2.49147 13.71 2.85863 13.8745 3.29748C14.0389 3.73633 14.044 4.21898 13.8889 4.66122C13.7338 5.10345 13.4283 5.47716 13.0258 5.71714C12.6233 5.95712 12.1492 6.04812 11.6864 5.97426C11.2236 5.9004 10.8015 5.66636 10.4937 5.31301L5.95765 7.58101C6.01683 7.8572 6.01683 8.14281 5.95765 8.419L10.4937 10.687C10.8133 10.3198 11.2558 10.0818 11.7384 10.0175C12.2209 9.95331 12.7103 10.0673 13.1149 10.3381C13.5194 10.6089 13.8113 11.018 13.9358 11.4886C14.0602 11.9592 14.0088 12.4591 13.7911 12.8945C13.5734 13.3299 13.2044 13.671 12.7532 13.8537C12.302 14.0365 11.7996 14.0485 11.3402 13.8873C10.8809 13.7262 10.4961 13.403 10.2579 12.9785C10.0198 12.5539 9.94465 12.057 10.0467 11.581L5.50865 9.313C5.24122 9.62029 4.88633 9.83858 4.49146 9.93868C4.09658 10.0388 3.68056 10.0159 3.29904 9.87314C2.91751 9.73037 2.58869 9.4745 2.35654 9.13976C2.12439 8.80502 2 8.40737 2 8.00001C2 7.59264 2.12439 7.19499 2.35654 6.86025C2.58869 6.52551 2.91751 6.26964 3.29904 6.12687C3.68056 5.98409 4.09658 5.96123 4.49146 6.06133C4.88633 6.16143 5.24122 6.37972 5.50865 6.68701L10.0447 4.41901C10.0156 4.28124 10.0012 4.1408 10.0017 4.00001Z" fill="#080827"></path>
                                    </svg>
                                </button>
                            </div>
                            <!-- Social End -->
                        </div>
                    </div>
                    <!-- Meta End -->
                </div>
                <!-- Content End -->
            </div>
        `;
        
        postsListContainer.appendChild(newsElement);
    });

    // Add Load More button if there are more items
    if (hasMoreItems) {
        const loadMoreDiv = document.createElement('div');
        loadMoreDiv.className = 'load-more-container';
        loadMoreDiv.innerHTML = `
            <!-- Load More Start -->
            <div class="flex justify-center mt-6">
                <button id="load-more-btn" 
                       data-page="${currentPage}"
                       data-read-list="${readList}"
                       class="px-6 py-2 bg-slate-900 hover:bg-slate-600 transition-all text-white text-sm font-medium">
                    Daha Fazla Göster
                </button>
            </div> <!-- Load More End -->
        `;
        postsListContainer.appendChild(loadMoreDiv);
        
        // Add event listener to the new button
        document.getElementById('load-more-btn').addEventListener('click', function() {
            const nextPage = parseInt(this.getAttribute('data-page')) + 1;
            const readListData = this.getAttribute('data-read-list');
            
            // Show loading state
            this.innerText = 'Yükleniyor...';
            this.disabled = true;
            
            // Fetch more items
            fetchReadListItems(readListData, nextPage);
        });
    }
}

function removeFromReadList(newsID) {
    let savedNewsIds = JSON.parse(localStorage.getItem('NewsIDs')) || [];

    savedNewsIds = savedNewsIds.filter(id => id !== newsID);

    localStorage.setItem('NewsIDs', JSON.stringify(savedNewsIds));

    if (savedNewsIds.length > 0) {
        fetchReadListItems(savedNewsIds.join(','), 1);
    } else {
        const postsListContainer = document.querySelector('.posts-list');
        if (postsListContainer) {
            postsListContainer.innerHTML = '<div class="text-center py-8">Henüz okuma listenizde haber bulunmamaktadır.</div>';
        }
    }
}