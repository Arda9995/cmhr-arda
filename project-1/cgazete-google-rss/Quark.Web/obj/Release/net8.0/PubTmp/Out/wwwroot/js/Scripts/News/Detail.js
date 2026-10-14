var newsOrder = 1;
var isLoadingNews = false;
var newsIds = document.getElementById('ScrollNewsIDs').value.split(",");
var currentNewsId = getLastNumberFromUrl(window.location.href);
let min = 2410000;
let max = 2412300;
if (currentNewsId === null || currentNewsId === undefined || isNaN(currentNewsId) || currentNewsId < 100000 || currentNewsId > 2700000) {
    currentNewsId = randomBetween(min, max);
}
var currentSlug = window.location.pathname.substring(1);
var newsDetailHeaderTitle = document.getElementById('news-detail-header-title');
var newsDetailMobileHeaderTitle = document.getElementById('news-detail-mobile-header-title');

var getCategory = document.getElementById('CategoryName');
var getNewsTypeID = document.querySelector('.news-detail-news-type');

function getLastNumberFromUrl(url) {
    // Sonunda sayý bulunan ve / veya ? karakterinden önce gelen kýsmý yakalar
    var match = new URL(url).pathname
    var urlNewsID = match.split('-')
    return urlNewsID[urlNewsID.length - 1] ? urlNewsID[urlNewsID.length - 1] : null;
}

function randomBetween(min, max) {
    // min ve max dahil, arada random bir tam sayý döner
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

const loadObserver = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting && newsOrder < 4) {
            if (!getNewsTypeID) {
                GetNextNewsDetail();
            }
        }
    });
}, {
    root: null,
    rootMargin: "500px",
    threshold: 0
});

let currentActiveSlug = currentSlug;

function updateActiveNewsOnScroll() {
    // Tüm haber kapsayýcýlarýný sýrayla al
    const containers = document.querySelectorAll('[id^="next-news-container-"]');
    if (!containers.length) return;

    let activeContainer = null;

    // Sondan baþa bak, ekrana en çok giren aþaðýdaki haberi bul
    for (let i = containers.length - 1; i >= 0; i--) {
        const container = containers[i];
        const newsItem = container.querySelector('.news-item');
        if (!newsItem) continue;

        const rect = container.getBoundingClientRect();
        // Eðer container'ýn üstü, ekranýn altýndan 100px yukarýda ise
        if (rect.top < window.innerHeight - 300) {
            activeContainer = container;
            break;
        }
    }

    // Eðer hiçbir kapsayýcý 100px çizgisine gelmemiþse, ilk haber aktif olsun
    if (!activeContainer) {
        activeContainer = containers[0];
        if (!activeContainer.querySelector('.news-item')) return;
    }

    const newsItem = activeContainer.querySelector('.news-item');
    let slug = newsItem.getAttribute('data-slug');
    slug = slug ? slug.replace(/^\//, '') : '';
    const titleElem = newsItem.querySelector('.news-detail-header-title');
    const newTitle = titleElem?.value || titleElem?.innerText || document.title;

    if (slug && slug !== currentActiveSlug) {
        currentActiveSlug = slug;
        currentSlug = slug;
        history.replaceState(null, null, '/' + slug);
        newsDetailHeaderTitle.innerText = newTitle;
        newsDetailMobileHeaderTitle.innerText = newTitle;
        document.title = newTitle;

        // Analytics tetikle
        const event = new Event('urlchange');
        window.dispatchEvent(event);
    }
}

// Performans için throttle fonksiyonu (100ms önerilir)
function throttle(fn, wait) {
    let last = 0;
    return function () {
        const now = Date.now();
        if (now - last >= wait) {
            fn();
            last = now;
        }
    }
}

// Scroll eventine baðla
window.addEventListener('scroll', throttle(updateActiveNewsOnScroll, 100));


let count = 1;

let sentinel = document.getElementById('next-news-container-' + newsOrder);
if (sentinel) {
    loadObserver.observe(sentinel);
}

function getRandomIdExcludingFirst(min, max, exclude) {
    let rand;
    do {
        rand = Math.floor(Math.random() * (max - min + 1)) + min;
    } while (rand === exclude);
    return rand;
}
function GetNextNewsDetail() {
    //var newsId = getRandomIdExcludingFirst(currentNewsId - 100, currentNewsId - 1, currentNewsId);
    var newsId = newsIds[newsOrder - 1];
    $.ajax({
        url: `/News/DetailNextNews/${newsId}`,
        type: 'GET',
        success: function (partialViewHtml) {
            var containerId = `#next-news-container-${newsOrder}`;
            $(containerId).html(partialViewHtml);

            

            newsOrder++;

            var newDiv = document.createElement('div');
            newDiv.id = `next-news-container-${newsOrder}`;
            document.getElementById('main').appendChild(newDiv);
            sentinel = null;
            sentinel = document.getElementById('next-news-container-' + newsOrder);
            if (sentinel && newsOrder < 4) {
                loadObserver.observe(sentinel);
            }
            isLoadingNews = false;
        },
        error: function () {
            isLoadingNews = false;
        }
    });
}


(function () {
    const METRICA_ID = 49053425; // <-- Buraya kendi Yandex Metrica ID'nizi yazýn

    function sendPageView(url) {
        // GA4'e page_view gönder
        if (typeof gtag === 'function') {
            gtag('event', 'page_view', {
                page_location: url,
                page_path: new URL(url).pathname,
                page_title: document.title
            });
            gtag('event', 'Infinity Scroll Page', {
                'event_category': 'Infinity Page View',
            });
        }

        // Yandex Metrica'ya hit gönder
        if (typeof ym === 'function') {
            ym(METRICA_ID, 'hit', url, {
                title: document.title
            });
        }
    }

    // pushState ve replaceState'i override et
    ['pushState', 'replaceState'].forEach(function (method) {
        const original = history[method];
        history[method] = function () {
            const result = original.apply(this, arguments);
            const event = new Event('urlchange');
            window.dispatchEvent(event);
            return result;
        };
    });

    // Olaylarý dinle
    window.addEventListener('popstate', () => sendPageView(location.href));
    window.addEventListener('urlchange', () => sendPageView(location.href));
})();