class StickyHeader {
    constructor(options = {}) {
        this.settings = {
            headerSelector: options.headerSelector || "header",
            stickyClass: options.stickyClass || "fixed",
            hiddenClass: options.hiddenClass || "!hidden",
            placeholderClass: options.placeholderClass || "placeholder-header",
            listSelectors: options.listSelectors || [
                ".bar",
                ".menu",
                ".top",
                ".news-details",
            ],
            breakpoint: options.breakpoint || 992, // Mobile breakpoint
            disableOnMobile: options.disableOnMobile || true,
        };

        this.header = document.querySelector(this.settings.headerSelector);

        if (!this.header) {
            console.error("Header element not found!");
            return;
        }

        this.headerHeight = null;

        this.init();
    }

    init() {
        window.addEventListener("load", () => this.handleScroll());
        if (window.innerWidth > this.settings.breakpoint) {
            window.addEventListener("scroll", () => this.handleScroll());
        }

        // Resize için
        window.addEventListener("resize", () => {
            if (window.innerWidth > this.settings.breakpoint) {
                window.addEventListener("scroll", () => this.handleScroll());
            } else if (this.settings.disableOnMobile) {
                this.resetHeader();
            }
        });
    }

    handleScroll() {
        if (this.headerHeight === null) {
            this.headerHeight = this.header.offsetHeight - 90;
        }

        if (window.scrollY >= this.headerHeight) {
            this.makeSticky();
        } else {
            this.resetHeader();
        }
    }

    makeSticky() {
        this.header.classList.add(
            this.settings.stickyClass,
            "top-0",
            "left-0",
            "right-0"
        );
        this.header.classList.remove("relative");

        this.settings.listSelectors.forEach((selector) => {
            const element = document.querySelector(selector);
            if (element) element.classList.add(this.settings.hiddenClass);
        });

        const stickyBar = this.header.querySelector(".sticky-bar");
        if (stickyBar) stickyBar.classList.remove("hidden");

        const newsDetails = this.header.querySelector(".news-details");
        if (newsDetails) newsDetails.classList.remove("hidden");

        // Placeholder ekle
        if (!document.querySelector(`.${this.settings.placeholderClass}`)) {
            const placeholder = document.createElement("div");
            placeholder.style.height = `${this.headerHeight}px`;
            placeholder.classList.add(this.settings.placeholderClass);
            this.header.parentNode.insertBefore(placeholder, this.header);
        }
    }

    resetHeader() {
        this.header.classList.remove(
            this.settings.stickyClass,
            "top-0",
            "left-0",
            "right-0"
        );
        this.header.classList.add("relative");

        this.settings.listSelectors.forEach((selector) => {
            const element = document.querySelector(selector);
            if (element) element.classList.remove(this.settings.hiddenClass);
        });

        const stickyBar = this.header.querySelector(".sticky-bar");
        if (stickyBar) stickyBar.classList.add("hidden");

        const newsDetails = this.header.querySelector(".news-details");
        if (newsDetails) newsDetails.classList.add("hidden");

        const placeholder = document.querySelector(
            `.${this.settings.placeholderClass}`
        );
        if (placeholder) placeholder.remove();
    }
}

const HoverPagination = (splide) => {
    return {
        setup() {
            if (!splide.options.pagination) return;

            const bindPaginationEvents = () => {
                const paginationItems = splide.root.querySelectorAll(
                    ".splide__pagination li"
                );

                paginationItems.forEach((item, index) => {
                    item.removeEventListener(
                        "mouseenter",
                        this.onPaginationHover
                    );
                    item.addEventListener(
                        "mouseenter",
                        this.onPaginationHover.bind(this, index)
                    );
                });
            };

            this.onPaginationHover = (index) => {
                splide.go(index);
            };

            splide.on("mounted", bindPaginationEvents);

            const originalAdd = splide.add.bind(splide);
            splide.add = (...args) => {
                originalAdd(...args);
                splide.refresh();
                bindPaginationEvents();
            };
        },
    };
};

const ProgressBar = (splide) => {
    return {
        setup() {
            this.progressContainer = document.createElement("div");
            this.progressContainer.className =
                "flex space-x-2 py-2 px-3 bg-neutral-100";
            this.progressContainer.classList.add("progress-bars");

            splide.root.parentNode.insertBefore(
                this.progressContainer,
                splide.root.nextSibling
            );
        },

        mount() {
            this.renderProgressBars();

            splide.on("slide:removed", (index) => {
                this.removeProgressBar(index);
            });

            splide.on("autoplay:playing", (rate) => {
                const activeIndex = splide.index;
                const roundedRate = Math.round(rate * 100);

                const activeProgress =
                    this.progressBars[activeIndex].querySelector(".progress");
                activeProgress.style.width = `${roundedRate}%`;

                this.progressBars.forEach((progressBar, index) => {
                    const progress = progressBar.querySelector(".progress");
                    if (index < activeIndex) {
                        progress.style.width = "100%";
                    } else if (index > activeIndex) {
                        progress.style.width = "0%";
                    }
                });
            });

            const originalAdd = splide.add.bind(splide);
            splide.add = (...args) => {
                originalAdd(...args);
                this.renderProgressBars();
            };

            this.addHoverPause();
        },

        renderProgressBars() {
            const slideCount = splide.length;

            this.progressContainer.innerHTML = "";

            for (let i = 0; i < slideCount; i++) {
                const progressBar = document.createElement("div");
                progressBar.className =
                    "progress-bar bg-gray-300 flex-1 h-2 rounded-full overflow-hidden cursor-pointer";
                progressBar.dataset.index = i;

                const progress = document.createElement("div");
                progress.className = "progress bg-red-500 h-full";
                progress.style.width = "0%";
                progressBar.appendChild(progress);

                progressBar.addEventListener("click", () => {
                    splide.go(i);
                });

                this.progressContainer.appendChild(progressBar);
            }

            this.progressBars =
                this.progressContainer.querySelectorAll(".progress-bar");
        },

        removeProgressBar(index) {
            const progressBar = this.progressBars[index];
            if (progressBar) {
                progressBar.remove();
                this.renderProgressBars();
            }
        },

        addHoverPause() {
            this.progressContainer.addEventListener("mouseenter", () => {
                splide.Components.Autoplay.pause();
            });

            this.progressContainer.addEventListener("mouseleave", () => {
                splide.Components.Autoplay.play();
            });
        },
    };
};

const IframeValidator = (splide) => {
    return {
        mount() {
            const slides = splide.Components.Elements.slides;

            slides.forEach((slide, index) => {
                const iframe = slide.querySelector("iframe");

                if (iframe) {
                    // Listen for iframe load event
                    iframe.addEventListener("load", () => {
                        try {
                            const iframeContent =
                                iframe.contentDocument ||
                                iframe.contentWindow.document;

                            // If iframe content is empty, remove the slide
                            if (
                                !iframeContent ||
                                !iframeContent.body ||
                                iframeContent.body.innerHTML.trim() === ""
                            ) {
                                console.warn("Iframe içeriği boş:", iframe.src);
                                this.removeSlide(index);
                            }
                        } catch (error) {
                            // Cross-origin or other access error
                            console.warn(
                                "Iframe erişilemedi veya hatalı:",
                                iframe.src,
                                error
                            );
                            this.removeSlide(index);
                        }
                    });

                    // Listen for iframe error event
                    iframe.addEventListener("error", () => {
                        console.error("Iframe yüklenme hatası:", iframe.src);
                        this.removeSlide(index);
                    });
                }
            });
        },

        /**
         * Remove a slide and refresh Splide.
         * @param {number} index - The index of the slide to remove.
         */
        removeSlide(index) {
            splide.remove(index);
            splide.emit("slide:removed", index);
        },
    };
};

const LinkPagination = (splide) => {
    return {
        mount() {
            const addLinkToPagination = (data) => {
                // Orijinal slide'ları al (klonlanmamış olanları)
                const originalSlides = splide.Components.Elements.slides.filter(
                    (slide) => !slide.classList.contains("splide__slide--clone")
                );

                data.items.forEach((item, index) => {
                    // Orijinal slide'ı al
                    const slide = originalSlides[index];
                    if (!slide) return;

                    const link = slide.querySelector("a");
                    if (!link) return;

                    const href = link.getAttribute("href");
                    if (!href) return;

                    // Butona href değerini data attribute olarak ekle
                    item.button.setAttribute("data-href", href);

                    // Mevcut click event listener'ları kaldır
                    const newButton = item.button.cloneNode(true);
                    item.button.parentNode.replaceChild(newButton, item.button);
                    item.button = newButton;

                    // Yeni click event listener ekle
                    item.button.addEventListener("click", (e) => {
                        // HoverPagination ile çakışmaması için timeout kullan
                        setTimeout(() => {
                            //window.location.href = href;
                            window.open(href, '_blank');
                        }, 50);
                    });
                });
            };

            // İlk mount edildiğinde
            splide.on("pagination:mounted", addLinkToPagination);

            // Slide eklendiğinde veya güncellendiğinde
            splide.on("pagination:updated", function (data) {
                addLinkToPagination(data);
            });
        },
    };
};

// Dropdown işlemleri
const initDropdowns = () => {
    document.querySelectorAll(".select-box").forEach((selectBox) => {
        const mainButton = selectBox.querySelector("button");
        const content = selectBox.querySelector(".dropdown-content");
        const toggle = selectBox.querySelector(".toggle");
        const optionButtons = content?.querySelectorAll("button");
        if (!mainButton || !content || !toggle || !optionButtons) return;
        // Ana butona tıklandığında
        mainButton.addEventListener("click", (e) => {
            e.stopPropagation();
            // Diğer tüm dropdownları kapat
            document.querySelectorAll(".select-box").forEach((otherBox) => {
                if (otherBox !== selectBox) {
                    const otherContent =
                        otherBox.querySelector(".dropdown-content");
                    const otherToggle = otherBox.querySelector(".toggle");
                    if (otherContent) otherContent.classList.add("hidden");
                    if (otherToggle) otherToggle.classList.remove("rotate-180");
                }
            });
            content.classList.toggle("hidden");
            toggle.classList.toggle("rotate-180");
        });
        // Seçenek butonlarına tıklandığında
        optionButtons.forEach((button) => {
            button.addEventListener("click", (e) => {
                e.stopPropagation();
                const value = button.getAttribute("data-value");
                const span = mainButton.querySelector("span");
                if (span && value) {
                    span.textContent = value;
                }
                // Dropdown'ı kapat
                content.classList.add("hidden");
                toggle.classList.remove("rotate-180");
            });
        });
    });
};

// Sayfa herhangi bir yerine tıklandığında dropdownları kapat
document.addEventListener("click", (e) => {
    const target = e.target;
    const dropdowns = document.querySelectorAll(".select-box");

    dropdowns.forEach((dropdown) => {
        if (!dropdown.contains(target)) {
            const content = dropdown.querySelector(".dropdown-content");
            const toggle = dropdown.querySelector(".toggle");
            if (content) content.classList.add("hidden");
            if (toggle) toggle.classList.remove("rotate-180");
        }
    });
});

// DOM yüklendiğinde dropdown'ları başlat
if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initDropdowns);
} else {
    initDropdowns();
}

/*if (document.querySelector("#header .crypto-slide")) {
    new Splide("#header .crypto-slide", {
        type: "loop",
        perPage: 1,
        drag: "free",
        focus: "center",
        pagination: false,
        lazyLoad: 'nearby',
        arrows: false,
        autoScroll: {
            speed: 0.5,
        },
    }).mount(window.splide.Extensions);
}
*//*
let secondSlider = null; // Global olarak tanımlı olmalı

function isSplideVisible(el) {
    if (!el) return false;
    const rect = el.getBoundingClientRect();
    return rect.top < window.innerHeight && rect.bottom > 0;
}

function handleSplideScroll(sliderElSelector, splideOptions = {}, mountOptions = undefined) {
    const sliderEl = document.querySelector(sliderElSelector);
    if (!sliderEl) return;

    if (isSplideVisible(sliderEl)) {
        if (!secondSlider) {
            console.log("Slider is visible, creating instance:", sliderElSelector);
            secondSlider = new Splide(sliderEl, splideOptions).mount(mountOptions);
        }
    } else {
        if (secondSlider) {
            console.log("Slider is hidden, destroying instance:", sliderElSelector);
            secondSlider.destroy(true);
            secondSlider = null;
        }
    }
}

// Scroll ile sürekli kontrol et
window.addEventListener('scroll', () => {
    handleSplideScroll('.second--slider', {
        arrows: true,
        type: "loop",
        pauseOnHover: true,
        focusOnHover: true,
        lazyLoad: 'nearby'
    }, { HoverPagination, LinkPagination });
});


// Sayfa yüklendiğinde de kontrol et
document.addEventListener('DOMContentLoaded', () => {
    handleSplideScroll('.second--slider', {
        arrows: true,
        type: "loop",
        pauseOnHover: true,
        focusOnHover: true,
        lazyLoad: 'nearby'
    }, { HoverPagination, LinkPagination });
});
*/
if (document.querySelector(".slider")) {
    window.slider = new Splide(".slider", {
        arrows: true,
        pagination: false,
        type: "loop",
        autoplay: true,
        interval: 30000,
        pauseOnHover: true,
        focusOnHover: true,
        lazyLoad: 'nearby'
    }).mount({ ProgressBar });
}

if (document.querySelector(".second--slider")) {
    window.secondSlider = new Splide(".second--slider", {
        arrows: true,
        type: "loop",
        pauseOnHover: true,
        focusOnHover: true,
        lazyLoad: 'nearby'
    }).mount({ HoverPagination, LinkPagination });
}

if (document.querySelector(".third--slider")) {
    window.thirdSlider = new Splide(".third--slider", {
        arrows: false,
        type: "loop",
        autoplay: true,
        interval: 10000,
        pauseOnHover: true,
        focusOnHover: true,
        lazyLoad: 'nearby'
        // autoHeight: true,
    }).mount({ HoverPagination, LinkPagination });
}

if (document.querySelector(".authors--slider")) {
    const authorSlider = new Splide(".authors--slider", {
        arrows: true,
        type: "loop",
        lazyLoad: 'nearby',
    }).mount();
}

if (document.querySelector(".second-progress-slider")) {
    const secondProgressSlider = new Splide(".second-progress-slider", {
        arrows: false,
        pagination: false,
        type: "loop",
        autoplay: true,
        interval: 5000,
        pauseOnHover: true,
        focusOnHover: true,
        lazyLoad: 'nearby',
    }).mount({ ProgressBar });
}

if (document.querySelector(".books.splide")) {
    const books = new Splide(".books.splide", {
        type: "loop",
        drag: "free",
        focus: "center",
        pagination: false,
        lazyLoad: 'nearby',
        arrows: false,
        fixedWidth: 82,
        gap: 10,
        autoScroll: {
            speed: 0.5,
        },
        // mediaQuery: 'min',
        // breakpoints: {
        // 	767: {
        // 		perPage: 4,
        // 	},
        // 	991: {
        // 		perPage: 6,
        // 	},
        // },
    }).mount(window.splide.Extensions);
}

if (document.querySelector(".info-line.splide")) {
    new Splide(".info-line.splide", {
        // type: "loop",
        perPage: 1,
        drag: "free",
        focus: "center",
        pagination: false,
        arrows: false,
        autoScroll: {
            speed: 1,
        },
        breakpoints: {
            991: {
                type: "loop",
            },
        },
    }).mount(window.splide.Extensions);
}

// News Tabs
const newsTabs = document.querySelectorAll(".news-tabs a");
const allContents = document.querySelectorAll(".tab-contents .tab");

newsTabs.forEach((tab) => {
    const id = tab.getAttribute("href").replace("#", "");
    const content = document.getElementById(id);

    tab.addEventListener("click", (e) => {
        newsTabs.forEach((item) => item.classList.remove("active"));
        tab.classList.add("active");

        allContents.forEach((item) => item.classList.add("hidden"));
        content.classList.remove("hidden");

        e.preventDefault();
    });
});

// Modal Link
const modalLinks = document.querySelectorAll(".modal-link, #toggleMenu");

modalLinks.forEach((link) => {
    link.addEventListener("click", (e) => {
        e.preventDefault();

        let modal = document.querySelector(".c-modal");

        if (link.id === "toggleMenu") {
            modal = document.querySelector("#hamburgerMenu");
        }

        modal.classList.toggle("-translate-y-full");
        modal.classList.toggle("opacity-0");
        modal.classList.toggle("pointer-events-none");

        const header = document.querySelector("#header");
        let newDiv = document.querySelector(".bg-reference-modal");

        if (!newDiv) {
            newDiv = document.createElement("div");
            newDiv.classList.add(
                "bg-reference-modal",
                "fixed",
                "inset-0",
                "bg-white/50"
            );
            newDiv.textContent = "";
            header.appendChild(newDiv);
        } else {
            header.removeChild(newDiv);
        }
    });
});

// Modal dışına tıklandığında kapat
document.addEventListener("click", (e) => {
    const modal = document.querySelector(".c-modal");
    const modalContent = modal?.querySelector(".modal-content");
    const bgModal = document.querySelector(".bg-reference-modal");
    const header = document.querySelector("#header");

    const hamburgerMenu = document.querySelector("#hamburgerMenu");
    const hamburgerContent = hamburgerMenu?.querySelector(".modal-content");

    // Eğer modal varsa ve tıklanan yer modal içeriği değilse
    if (
        modal &&
        !modalContent?.contains(e.target) &&
        !e.target.closest(".modal-link")
    ) {
        modal.classList.add(
            "-translate-y-full",
            "opacity-0",
            "pointer-events-none"
        );

        // Arka plan modalını kaldır
        if (bgModal && header) {
            header.removeChild(bgModal);
        }
    }

    if (
        hamburgerMenu &&
        !hamburgerContent?.contains(e.target) &&
        !e.target.closest("#toggleMenu")
    ) {
        hamburgerMenu.classList.add(
            "-translate-y-full",
            "opacity-0",
            "pointer-events-none"
        );

        if (bgModal && header) {
            header.removeChild(bgModal);
        }
    }
});

/**
 *
 * @param {*} img
 * @param {*} target
 * @param {*} index
 * @param {*} sliderName
 * @param {*} sponsorPosition
 */
const avvamobile_ad = (img, target, index, sliderName, sponsorPosition) => {
    index = index - 1;

    const sliderNameList = [
        { name: "ust", slider: slider },
        { name: "orta", slider: secondSlider },
        { name: "sag", slider: thirdSlider },
    ];

    const findSlider = sliderNameList.find(
        (item) => item.name === sliderName
    ).slider;

    const hasProgressBar = findSlider.Components.ProgressBar !== undefined;

    if (hasProgressBar) {
        const parentElement = findSlider.root.parentNode;
        const progressBars = parentElement.querySelector(".progress-bars");

        if (progressBars) {
            const observer = new MutationObserver(() => {
                const progressBarItem = progressBars.querySelector(
                    `[data-index="${index}"]`
                );
                if (
                    progressBarItem &&
                    !progressBarItem.hasAttribute("data-link")
                ) {
                    progressBarItem.setAttribute("data-link", target);

                    progressBarItem.addEventListener("click", () => {
                        window.open(target, "_blank");
                    });
                }
            });

            observer.observe(progressBars, { childList: true, subtree: true });
        }
    } else {
        findSlider.on("pagination:mounted", function (data) {
            data.items.forEach((item, indexNum) => {
                if (indexNum === index) {
                    item.button.setAttribute("data-link", target);

                    item.button.addEventListener("click", () => {
                        window.open(target, "_blank");
                    });
                }
            });
        });
    }

    const sliderElement = findSlider.root;

    const rect = sliderElement.querySelector("li").getBoundingClientRect();
    const height = rect.height;

    findSlider.add(
        `<li class="splide__slide relative">
		<span class="absolute top-5 ${sponsorPosition == "sag" ? "right-5" : "left-5"
        }  bg-white/50 rounded text-black text-xs font-bold py-1.5 px-3">Sponsor</span>
		<a target="_blank" rel="nofollow" href="${target}"><img style="height: ${height}px" class="object-cover w-full" src="${img}" alt="" /></a></li>`,
        index
    );
};

window.avvamobile_ad = avvamobile_ad;

if (document.querySelector(".mobile-top-slider")) {
    window.mobileTopSlider = new Splide(".mobile-top-slider", {
        arrows: false,
        type: "loop",
    }).mount();
}

if (document.querySelector(".mobile-cryptos-area")) {
    new Splide(".mobile-cryptos-area", {
        type: "loop",
        perPage: 4,
        drag: "free",
        // focus: "center",
        autoWidth: true,
        gap: "20px",
        pagination: false,
        arrows: false,
        autoScroll: {
            speed: 1,
        },
    }).mount(window.splide.Extensions);
}

if (document.querySelector(".mobile-second-slider")) {
    window.mobileSecondSlider = new Splide(".mobile-second-slider", {
        arrows: false,
        pagination: false,
        type: "loop",
        autoplay: true,
        interval: 5000,
        pauseOnHover: false,
        focusOnHover: false,
    }).mount({ IframeValidator, ProgressBar });
}

if (document.querySelector(".mobile-big-slider")) {
    new Splide(".mobile-big-slider", {
        arrows: false,
        pagination: false,
        type: "loop",
        fixedWidth: 210,
        gap: 20,
        padding: { left: 20 },
        drag: true,
        flickPower: 100, // Sürükleme hızını azaltır (varsayılan: 600)
        flickMaxPages: 1, // Tek seferde kaç sayfa ilerleyebileceğini sınırla
        speed: 800, // Geçiş animasyonunu yavaşlatır (varsayılan: 400ms)
    }).mount();
}

const stickyHeader = new StickyHeader({
    headerSelector: "#header",
    breakpoint: 768,
    disableOnMobile: true,
    listSelectors: [".bar", ".menu", ".top"],
});

// SearchPageForm ID'sindeki formun içindeki input'a bir şey yazıldığında
// SearchPageResults ID'sindeki hidden classını kaldır eğer komple silinirde tekrar hidden ekle
const searchInput = document.querySelector("#searchPageForm input");
const searchResults = document.querySelector("#searchPageResults");

searchInput?.addEventListener("input", () => {
    if (searchInput.value) {
        searchResults?.classList.remove("hidden");
    } else {
        searchResults?.classList.add("hidden");
    }
});

document.addEventListener("scroll", () => {
    const progressBar = document.querySelectorAll(
        ".progress-page-area .progress"
    );
    if (!progressBar) return; // Progress bar yoksa fonksiyondan çık

    progressBar.forEach((item) => {
        const scrollTop = window.scrollY;
        const scrollHeight = document.documentElement.scrollHeight;
        const clientHeight = document.documentElement.clientHeight;

        const scrollPercent = (scrollTop / (scrollHeight - clientHeight)) * 100;

        item.style.width = scrollPercent + "%";
    });
});


function adjustFontSize(newsId, change) {
    const titles = document.querySelectorAll(`.title[data-title-news-id='${newsId}']`);
    const descriptions = document.querySelectorAll(`.description[data-news-id='${newsId}']`);
    const containers = document.querySelectorAll(`.text-content[data-news-id='${newsId}']`);

    [...titles, ...descriptions, ...containers].forEach(el => {
        const currentSize = parseInt(window.getComputedStyle(el).fontSize);
        el.style.fontSize = `${currentSize + change}px`;
    });
}

document.querySelectorAll("a[data-text]").forEach((button) => {
    button.addEventListener("click", function (event) {
        event.preventDefault();

        //const textContentDiv = document.querySelector(".text-content");
        //const title = document.querySelector(".title");
        //const description = document.querySelector(".description");

        const mobileDescription = document.querySelector(".description.mobile");

        const titles = Array.from(document.querySelectorAll('.title[data-title-news-id]'))
            .filter(el => el.getAttribute('data-title-news-id')?.trim() !== '');

        const descriptions = Array.from(document.querySelectorAll('.description[data-subtitle-news-id]'))
            .filter(el => el.getAttribute('data-subtitle-news-id')?.trim() !== '');

        const containers = Array.from(document.querySelectorAll('.text-content[data-content-news-id]'))
            .filter(el => el.getAttribute('data-content-news-id')?.trim() !== '');


        const currentFontSize = containers.length > 0
            ? parseInt(window.getComputedStyle(containers[0]).fontSize)
            : null;

        const currentTitleSize = titles.length > 0
            ? parseInt(window.getComputedStyle(titles[0]).fontSize)
            : null;

        const currentDescSize = descriptions.length > 0
            ? parseInt(window.getComputedStyle(descriptions[0]).fontSize)
            : null;
        //const currentMobileDescSize = parseInt(
        //    window.getComputedStyle(mobileDescription).fontSize
        //);
        const action = this.getAttribute("data-text");

        let newFontSize = currentFontSize;
        let newTitleSize = currentTitleSize;
        let newDescSize = currentDescSize;

        if (action === "up" && currentFontSize < 40) {
            newFontSize += 2;
            newTitleSize += 2;
            newDescSize += 2;
        } else if (action === "down" && currentFontSize > 12) {
            newFontSize -= 2;
            newTitleSize -= 2;
            newDescSize -= 2;
        }

        // Uygula: Tüm içerik alanlarına (metin içerikleri)
        containers.forEach(container => {
            container.style.fontSize = `${newFontSize}px`;
        });

        // Uygula: Tüm başlıklara
        titles.forEach(title => {
            title.style.fontSize = `${newTitleSize}px`;
        });

        // Uygula: Tüm açıklamalara
        descriptions.forEach(description => {
            description.style.fontSize = `${newDescSize}px`;
        });
        //mobileDescription.style.fontSize = `${newMobileDescSize}px`;
    });
});

const shareButtons = document.querySelectorAll("#shareButton");

shareButtons.forEach((button) => {
    button?.addEventListener("click", async (e) => {
        e.preventDefault();

        const button = e.currentTarget; // Tıklanan buton
        const title = button.dataset.shareTitle; // data-title
        const text = button.dataset.shareText; // data-text
        const url = button.dataset.shareUrl; // data-url

        if (navigator.share) {
            try {
                await navigator.share({
                    title,
                    text,
                    url,
                });
                console.log("Paylaşım başarılı!");
            } catch (error) {
                console.error("Paylaşım sırasında hata oluştu:", error);
            }
        } else {
            alert("Cihazınız bu özelliği desteklemiyor.");
        }
    });
});

if (document.querySelector(".authors-new-slider")) {
    const authorSlider = new Splide(".authors-new-slider", {
        arrows: true,
        type: "loop",
        perPage: 6,
        pagination: false,
        breakpoints: {
            479: {
                perPage: 3,
            },
            767: {
                perPage: 4,
            },
            991: {
                perPage: 5,
            },
        },
    }).mount();
}

const selectPostButton = document.querySelector("#selectPost");
const SelectPostButtonIcon = selectPostButton?.querySelector("svg");
const deletePostButton = document.querySelector("#deletePost");
const selectPosts = document.querySelectorAll(".posts-list .item");
// const shareLinkContainer = document.querySelectorAll(".share-link-container");

selectPostButton?.addEventListener("click", (e) => {
    deletePostButton.classList.toggle("hidden");

    const buttonText = selectPostButton.querySelector("span");
    if (buttonText) {
        buttonText.textContent =
            buttonText.textContent === "Seç" ? "İptal" : "Seç";
    }

    selectPosts.forEach((item) => {
        const checkbox = item.querySelector('input[type="checkbox"]');
        const shareLinkContainer = item.querySelector(".share-link-container");

        checkbox.classList.toggle("hidden");
        shareLinkContainer.classList.toggle("hidden");
    });
});

const openDropdown = document.querySelector("#filterDropdown #openDropdown");
const dropdown = document.querySelector("#filterDropdown .dropdown-list");

openDropdown?.addEventListener("click", () => {
    dropdown.classList.toggle("hidden");
});

const selectFilterButton = document.querySelector("#selectFilter");
const filterDropdown = document.querySelector("#filterDropdown");
const dropdownOverlay = document.querySelector(".dropdown-overlay");

selectFilterButton?.addEventListener("click", () => {
    dropdownOverlay.classList.toggle("pointer-events-none");
    dropdownOverlay.classList.toggle("select-none");
    dropdownOverlay.classList.toggle("opacity-0");
    filterDropdown.classList.toggle("hidden");
});

const citySortButton = document.querySelector("#citySortButton");
const cityListContainer = document.querySelector(".city-list");
let isReversed = false;

citySortButton?.addEventListener("click", () => {
    const cities = Array.from(document.querySelectorAll(".city-list .city"));
    const reversedCities = [...cities].reverse();

    if (cityListContainer) {
        cityListContainer.innerHTML = "";
        reversedCities.forEach((city) => cityListContainer.appendChild(city));
    }

    isReversed = !isReversed;
});

const surveyList = document.querySelectorAll(".survey-list .item");

surveyList.forEach((item) => {
    item.addEventListener("click", () => {
        const check = item.querySelector(".check");
        const result = item.querySelector(".result");

        surveyList.forEach((el) => {
            el.querySelector(".result").classList.remove("hidden");
            el.classList.remove("active");
            el.classList.add("pointer-events-none");
        });

        item.classList.add("active");

        check.classList.remove("border-[#6B6B7D]");
        check.classList.add("border-[#FF8800]");
        check.querySelector(".checked").classList.remove("opacity-0");

        result.classList.remove("text-[#FF8800]");
        result.classList.add("text-white", "bg-[#FF8800]");

        const progressBars = document.querySelectorAll(
            ".survey-list .item .progress"
        );
        const totalPercentage = 100;
        let remainingPercentage = totalPercentage;
        let percentages = [];

        progressBars.forEach((bar, index) => {
            if (index === progressBars.length - 1) {
                percentages.push(remainingPercentage);
            } else {
                const randomPercentage = Math.floor(
                    Math.random() *
                    (remainingPercentage -
                        (progressBars.length - index - 1))
                );
                percentages.push(randomPercentage);
                remainingPercentage -= randomPercentage;
            }
        });

        percentages = percentages.sort(() => Math.random() - 0.5);

        progressBars.forEach((bar, index) => {
            bar.style.width = `${percentages[index]}%`;
            bar
                .closest(".item")
                .querySelector(
                    ".result"
                ).textContent = `${percentages[index]}%`;
        });
    });
});

// const currencySelect = document.querySelectorAll(".currency-select");

// currencySelect.forEach((item) => {
// 	const button = item.querySelector(".select-button");
// 	const toggle = item.querySelector(".toggle");
// 	const dropdown = item.querySelector(".dropdown");

// 	button.addEventListener("click", () => {
// 		toggle.classList.toggle("rotate-180");
// 		dropdown.classList.toggle("hidden");
// 	});

// 	dropdown.querySelectorAll("button").forEach((dropdownItem) => {
// 		dropdownItem.addEventListener("click", () => {
// 			// button.querySelector("span").innerHTML = dropdownItem.innerHTML;
// 			// dropdown.classList.add("hidden");
// 			toggle.classList.remove("rotate-180");
// 		});
// 	});
// });

const currencySelect = document.querySelector(".currency-select");
const currencyButton = document.querySelector(".currency-select button");
const currencySVG = document.querySelector(".currency-select button svg");
const currencyDropdown = document.querySelector(".currency-select .dropdown");

currencySelect?.addEventListener("click", () => {
    currencyDropdown.classList.toggle("hidden");
    currencySVG.classList.toggle("rotate-180");
});

const currencyForm = document.querySelector("#currencyForm");
const submitButton = document.querySelector("#submit");

submitButton?.addEventListener("click", (e) => {
    e.preventDefault();

    const result = document.querySelector("#resultScreen");
    const currencyType = document.querySelector("#currencyType");
    const valueType = document.querySelector("#valueType");

    result.classList.remove("hidden");
    // currencyType.classList.remove("hidden");
    // valueType.classList.add("hidden");
});

currencyForm?.addEventListener("submit", (e) => {
    e.preventDefault();
});

const currencyInput = document.querySelector(".currency-input");

currencyInput?.addEventListener("input", (e) => {
    const value = e.target.value;
    if (value.length > 0) {
        submitButton.classList.add("!bg-[#080827]", "!text-white");
    } else {
        submitButton.classList.remove("!bg-[#080827]", "!text-white");
    }
});

const mobileMenuSpecialLink = document.querySelector("#avva_special_link");
const mobileMenuSpecialDropdown = document.querySelector(".dropdown-list");

mobileMenuSpecialLink?.addEventListener("click", (e) => {
    e.preventDefault();

    const activeClass = [
        "bg-gradient-to-r",
        "from-transparent",
        "via-[#FCECEC]",
        "to-transparent",
        "active",
    ];

    if (mobileMenuSpecialLink.classList.contains("active")) {
        mobileMenuSpecialLink.classList.remove(...activeClass);
    } else {
        mobileMenuSpecialLink.classList.add(...activeClass);
    }

    mobileMenuSpecialDropdown.classList.toggle("hidden");
});

const toggleMobileMenu = document.querySelectorAll("#toggleMenu");
const mobileMenuOverlay = document.querySelector("#siteMenu .overlay");
const mobileSlideMenu = document.querySelector("#siteMenu #mobileSlideMenu");
const siteMenu = document.querySelector("#siteMenu");

toggleMobileMenu.forEach((item) => {
    item?.addEventListener("click", (e) => {
        e.preventDefault();

        siteMenu.classList.toggle("pointer-events-none");
        siteMenu.classList.toggle("select-none");

        mobileMenuOverlay.classList.toggle("opacity-0");
        mobileSlideMenu.classList.toggle("translate-x-full");
    });
});

document.addEventListener("DOMContentLoaded", () => {
    const header = document.querySelector("#mobileHeaderArea #header");
    const placeholder = document.getElementById("placeholder");
    const main = document.getElementById("contentArea");
    const postHeader = document.getElementById("postHeader");

    const headerHeight = header?.offsetHeight;
    let lastScrollY = 0;

    window.addEventListener("scroll", () => {
        const scrollY = window.scrollY;
        const mainTop = main?.offsetTop;

        if (scrollY >= mainTop) {
            placeholder?.classList.remove("hidden");
            placeholder.style.height = `${headerHeight}px`;

            if (scrollY > lastScrollY) {
                postHeader?.classList.remove("-translate-y-full");
                header?.classList.add("translate-y-full");
            } else {
                postHeader?.classList.add("-translate-y-full");
                header?.classList.remove("-translate-y-full");
                header?.classList.add("fixed", "top-0", "left-0", "right-0");
            }
        } else {
            header?.classList.remove("fixed", "-translate-y-full");
            placeholder?.classList.add("hidden");
            placeholder.style.height = "0px";

            postHeader?.classList.add("-translate-y-full");
        }

        lastScrollY = scrollY;
    });
});

const checkboxContainer = document.querySelectorAll(".checkbox-container");

checkboxContainer.forEach((item) => {
    const svg = item.querySelectorAll("svg");
    const label = item.querySelector("label");

    svg.forEach((item) => {
        item.addEventListener("click", () => {
            label.click();
        });
    });
});

// Header scroll behavior
document.addEventListener("DOMContentLoaded", () => {
    const header = document.querySelector(".mobile-header");
    let lastScrollY = window.scrollY;

    window.addEventListener("scroll", () => {
        const currentScrollY = window.scrollY;

        // Scroll yönünü belirle
        if (currentScrollY > lastScrollY && currentScrollY > 200) {
            // Aşağı scroll
            header?.classList.add("-translate-y-full");
        } else {
            // Yukarı scroll
            header?.classList.remove("-translate-y-full");
        }

        lastScrollY = currentScrollY;
    });
});

var turkishMonths = [
    "Ocak",
    "Şubat",
    "Mart",
    "Nisan",
    "Mayıs",
    "Haziran",
    "Temmuz",
    "Ağustos",
    "Eylül",
    "Ekim",
    "Kasım",
    "Aralık",
];

if (document.querySelector("#datepicker")) {
    var datepicker = new Datepicker("#datepicker", {
        ranged: true,
        separator: " - ",
        fromValue: function (str) {
            var dates = str.split(" - ");
            return dates.map(function (dateStr) {
                var parts = dateStr.split(".");
                return new Date(parts[2], parts[1] - 1, parts[0]);
            });
        },
    });
}

if (document.querySelector("#datepicker-without-range")) {

    var datepicker = new Datepicker("#datepicker-without-range", {
        ranged: false,
        separator: " - ",
        fromValue: function (str) {
            var dates = str.split(" - ");
            return dates.map(function (dateStr) {
                var parts = dateStr.split(".");
                return new Date(parts[2], parts[1] - 1, parts[0]);
            });
        },
    });
}

// function openSharePopup(url) {
//     window.open(url, "_blank", "width=600,height=400");
// }

document.querySelectorAll(".share-btn").forEach((button) => {
    button.addEventListener("click", function (event) {
        event.preventDefault(); // Linkin tam sayfa açılmasını engelle

        let platform = this.getAttribute("data-platform");
        let title = this.getAttribute("data-title") || "";
        let url = this.getAttribute("data-url") || window.location.href;
        let username = this.getAttribute("data-username") || "";
        let related = this.getAttribute("data-related") || "";

        let shareUrl = "";

        switch (platform) {
            case "twitter":
                shareUrl = `https://twitter.com/intent/tweet?text=${encodeURIComponent(
                    title
                )}&url=${encodeURIComponent(url)}`;
                if (username) {
                    shareUrl += `&via=${encodeURIComponent(username)}`;
                }
                if (related) {
                    shareUrl += `&related=${encodeURIComponent(related)}`;
                }
                break;
            case "whatsapp":
                shareUrl = `https://wa.me/?text=${encodeURIComponent(
                    title + " " + url
                )}`;
                break;
            case "facebook":
                shareUrl = `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(
                    url
                )}`;
                break;
            case "linkedin":
                shareUrl = `https://www.linkedin.com/sharing/share-offsite/?url=${encodeURIComponent(
                    url
                )}`;
                break;
        }

        if (shareUrl) {
            window.open(shareUrl, "_blank", "width=600,height=400");
        }
    });
});

const shareDropdown = document.querySelectorAll(".share-dropdown");

shareDropdown.forEach((dropdown) => {
    const shareDropdownButton = dropdown.querySelector("#shareDropdownButton");
    const shareDropdownContent = dropdown.querySelector(".content");

    shareDropdownButton.addEventListener("click", (e) => {
        e.stopPropagation();
        shareDropdownContent.classList.toggle("hidden");
    });

    // Close dropdown when clicking outside
    document.addEventListener("mouseup", (e) => {
        if (
            !dropdown.contains(e.target) &&
            !shareDropdownContent.classList.contains("hidden")
        ) {
            shareDropdownContent.classList.add("hidden");
        }
    });

    // Close dropdown when pressing escape key
    document.addEventListener("keydown", (e) => {
        if (
            e.key === "Escape" &&
            !shareDropdownContent.classList.contains("hidden")
        ) {
            shareDropdownContent.classList.add("hidden");
        }
    });
});

let photos = document.querySelectorAll("#photoGallery .photo");
let options = {
    root: null,
    rootMargin: "0px 0px -90% 0px",
    threshold: 0,
};

let visiblePhotos = new Set();

let observer = new IntersectionObserver((entries) => {
    entries.forEach((entry) => {
        if (entry.isIntersecting) {
            visiblePhotos.add(entry.target.id);
            history.replaceState(null, "", "#" + entry.target.id);
        } else {
            visiblePhotos.delete(entry.target.id);
        }
    });

    if (visiblePhotos.size === 0) {
        history.replaceState(null, "", location.pathname);
    }
}, options);

photos.forEach((photo) => observer.observe(photo));

const links = document.querySelectorAll(".author-links a");
const posts = document.querySelectorAll(".author-posts .post");

links.forEach((link) => {
    link.addEventListener("click", function (event) {
        event.preventDefault();

        const filter = this.getAttribute("href").replace("#", "");

        links.forEach((l) => {
            l.classList.remove("!text-black");
        });

        link.classList.add("!text-black");

        posts.forEach((post) => {
            const postFilter = post.getAttribute("data-filter");

            if (filter === "Tümü") {
                post.style.display = "flex";
            } else if (postFilter.startsWith(filter)) {
                post.style.display = "flex";
            } else {
                post.style.display = "none";
            }
        });
    });
});

const azButton = document.querySelector("#az-za");

azButton?.addEventListener("click", () => {
    const posts = document.querySelectorAll(".author-posts .post");
    const postsContainer = posts[0].parentElement;
    let isAscending = azButton.getAttribute("data-sort") !== "asc";

    // Convert NodeList to Array for sorting
    const postsArray = Array.from(posts);

    postsArray.sort((a, b) => {
        const filterA = a.getAttribute("data-filter").charAt(0);
        const filterB = b.getAttribute("data-filter").charAt(0);

        if (isAscending) {
            return filterA.localeCompare(filterB, "tr");
        } else {
            return filterB.localeCompare(filterA, "tr");
        }
    });

    // Clear container
    postsContainer.innerHTML = "";

    // Append sorted posts
    postsArray.forEach((post) => {
        postsContainer.appendChild(post);
    });

    // Toggle sort direction
    azButton.setAttribute("data-sort", isAscending ? "asc" : "desc");
});

const initDesktopMenu = () => {
    const menuButtons = document.querySelectorAll(
        '[data-toggle="desktop-menu"]'
    );
    const desktopMenu = document.querySelector("#desktopMenu");
    const menuBackdrop = document.querySelector("#menuBackdrop");
    const closeButton = desktopMenu?.querySelector(".close-drawer");

    // Menü kategorilerini başlat
    const initCategories = () => {
        const categories = document.querySelectorAll(".drawer-category");

        categories.forEach((category) => {
            category.addEventListener("click", () => {
                const submenu = category.nextElementSibling;
                const icon = category.querySelector(".category-icon");

                if (submenu && icon) {
                    // Diğer tüm alt menüleri kapat
                    categories.forEach((otherCategory) => {
                        if (otherCategory !== category) {
                            const otherSubmenu =
                                otherCategory.nextElementSibling;
                            const otherIcon =
                                otherCategory.querySelector(".category-icon");
                            if (otherSubmenu && otherIcon) {
                                otherSubmenu.classList.add("hidden");
                                otherIcon.classList.remove("rotate-180");
                            }
                        }
                    });

                    // Tıklanan kategoriyi aç/kapat
                    submenu.classList.toggle("hidden");
                    icon.classList.toggle("rotate-180");
                }
            });
        });
    };

    // Menüyü aç
    const openMenu = () => {
        desktopMenu?.classList.remove("translate-x-full");
        menuBackdrop?.classList.remove("opacity-0", "pointer-events-none");
        document.body.style.overflow = "hidden";
    };

    // Menüyü kapat
    const closeMenu = () => {
        desktopMenu?.classList.add("translate-x-full");
        menuBackdrop?.classList.add("opacity-0", "pointer-events-none");
        document.body.style.overflow = "";

        // Tüm alt menüleri kapat
        const categories = document.querySelectorAll(".drawer-category");
        categories.forEach((category) => {
            const submenu = category.nextElementSibling;
            const icon = category.querySelector(".category-icon");
            if (submenu && icon) {
                submenu.classList.add("hidden");
                icon.classList.remove("rotate-180");
            }
        });
    };

    // Event Listeners
    menuButtons?.forEach((button) => {
        button.addEventListener("click", (e) => {
            e.preventDefault();
            openMenu();
        });
    });

    closeButton?.addEventListener("click", () => {
        closeMenu();
    });

    menuBackdrop?.addEventListener("click", () => {
        closeMenu();
    });

    // ESC tuşu ile kapatma
    document.addEventListener("keydown", (e) => {
        if (
            e.key === "Escape" &&
            !desktopMenu?.classList.contains("translate-x-full")
        ) {
            closeMenu();
        }
    });

    // Kategori işlemlerini başlat
    initCategories();
};

// DOM yüklendiğinde desktop menü işlemlerini başlat
if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initDesktopMenu);
} else {
    initDesktopMenu();
}

//
const initHeaderAd = () => {
    const headerAdArea = document.getElementById("headerAdArea");

    if (!headerAdArea) return;

    // Create placeholder element
    const placeholder = document.createElement("div");
    placeholder.style.display = "none";
    placeholder.style.height = headerAdArea.offsetHeight + "px";
    headerAdArea.parentNode.insertBefore(placeholder, headerAdArea);

    let hasBeenFixed = false;

    const handleScroll = () => {
        if (window.scrollY > 150 && !hasBeenFixed) {
            headerAdArea.classList.add(
                "fixed",
                "top-10",
                "left-0",
                "right-0",
                "z-50",
                "bg-black/50",
                "py-5"
            );
            placeholder.style.display = "block";

            setTimeout(() => {
                headerAdArea.classList.remove(
                    "fixed",
                    "top-10",
                    "left-0",
                    "right-0",
                    "z-50",
                    "bg-black/50",
                    "py-5"
                );
                placeholder.style.display = "none";
                hasBeenFixed = true;
            }, 7000);
        } else if (window.scrollY <= 150) {
            headerAdArea.classList.remove(
                "fixed",
                "top-10",
                "left-0",
                "right-0",
                "z-50",
                "bg-black/50",
                "py-5"
            );
            placeholder.style.display = "none";
        }
    };

    window.addEventListener("scroll", handleScroll);
};

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initHeaderAd);
} else {
    initHeaderAd();
}

// Pull-to-refresh özelliğinin her yerde çalışmasını sağla
const enablePullToRefresh = () => {
    // iOS Safari'de pull-to-refresh özelliğini etkinleştirmek için
    // sayfanın en üstünde biraz boşluk bırak
    const addPullToRefreshSpace = () => {
        // Sadece iOS cihazlarda çalıştır
        const isIOS =
            /iPad|iPhone|iPod/.test(navigator.userAgent) && !window.MSStream;

        if (isIOS) {
            // Sayfanın en üstüne stilize edilmiş bir boşluk ekle
            const refreshIndicator = document.createElement("div");
            refreshIndicator.style.height = "0px";
            refreshIndicator.style.overflow = "hidden";
            refreshIndicator.style.transition = "height 0.3s";
            refreshIndicator.id = "pull-to-refresh-indicator";

            document.body.insertBefore(
                refreshIndicator,
                document.body.firstChild
            );

            // Scroll olayını izle
            let startY = 0;
            document.addEventListener(
                "touchstart",
                (e) => {
                    startY = e.touches[0].pageY;
                },
                { passive: true }
            );

            document.addEventListener(
                "touchmove",
                (e) => {
                    const y = e.touches[0].pageY;
                    // Sadece sayfanın en üstündeyken ve aşağı çekerken
                    if (window.scrollY <= 0 && y > startY) {
                        refreshIndicator.style.height =
                            Math.min((y - startY) / 2, 60) + "px";
                    }
                },
                { passive: true }
            );

            document.addEventListener(
                "touchend",
                () => {
                    // Kullanıcı parmağını kaldırdığında
                    if (refreshIndicator.offsetHeight > 50) {
                        // Yeterince çekildiyse sayfayı yenile
                        window.location.reload();
                    } else {
                        // Değilse indikatörü kapat
                        refreshIndicator.style.height = "0px";
                    }
                },
                { passive: true }
            );
        }
    };

    // DOM yüklendiğinde çalıştır
    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", addPullToRefreshSpace);
    } else {
        addPullToRefreshSpace();
    }
};

// Pull-to-refresh özelliğini başlat
enablePullToRefresh();

// Subscription form handling
const initSubscriptionForm = () => {
    const form = document.querySelector(".max-w-lg form");
    const termsCheckbox = document.querySelector("#terms");
    const selectBox = document.querySelector(".select-box button");
    const dropdownContent = document.querySelector(".dropdown-content");
    const planButtons = document.querySelectorAll(".dropdown-content button");
    const toggle = document.querySelector(".toggle");

    if (form && termsCheckbox) {
        // Plan selection
        selectBox?.addEventListener("click", () => {
            dropdownContent?.classList.toggle("hidden");
            toggle?.classList.toggle("rotate-180");
        });

        // Close dropdown when clicking outside
        document.addEventListener("click", (e) => {
            const target = e.target;
            if (selectBox && !selectBox.contains(target)) {
                dropdownContent?.classList.add("hidden");
                toggle?.classList.remove("rotate-180");
            }
        });

        // Plan button clicks
        planButtons.forEach((button) => {
            button.addEventListener("click", () => {
                const value = button.getAttribute("data-value");
                const period =
                    button.querySelector(".text-gray-500")?.textContent;

                if (selectBox && value && period) {
                    const mainButtonText =
                        selectBox.querySelector("span:first-child");
                    const mainButtonPeriod =
                        selectBox.querySelector("span:nth-child(2)");

                    if (mainButtonText && mainButtonPeriod) {
                        mainButtonText.textContent = value + " ₺";
                        mainButtonPeriod.textContent = period;
                    }
                }

                dropdownContent?.classList.add("hidden");
                toggle?.classList.remove("rotate-180");
            });
        });

        // Form submission
        form.addEventListener("submit", (e) => {
            e.preventDefault();
            if (
                termsCheckbox instanceof HTMLInputElement &&
                !termsCheckbox.checked
            ) {
                alert("Lütfen şartları ve koşulları kabul edin.");
                return;
            }
            // Form submission logic
            const formData = new FormData(form);
            console.log("Form submitted", Object.fromEntries(formData));
        });
    }
};

// Initialize subscription form if it exists
if (document.querySelector(".max-w-lg form")) {
    initSubscriptionForm();
}

// Mobile menu toggle
document.addEventListener("DOMContentLoaded", () => {
    const menuButton = document.querySelector('button[aria-label="Ana menü"]');
    const mobileMenu = document.getElementById("mobile-menu");

    if (menuButton && mobileMenu) {
        menuButton.addEventListener("click", () => {
            const isExpanded =
                menuButton.getAttribute("aria-expanded") === "true";
            menuButton.setAttribute("aria-expanded", !isExpanded);
            mobileMenu.classList.toggle("hidden");
        });

        // Close mobile menu when clicking outside
        document.addEventListener("click", (e) => {
            if (
                !menuButton.contains(e.target) &&
                !mobileMenu.contains(e.target)
            ) {
                menuButton.setAttribute("aria-expanded", "false");
                mobileMenu.classList.add("hidden");
            }
        });
    }
});

// Ödeme sayfası işlemleri
const initPaymentForm = () => {
    const form = document.querySelector("form");
    const cardNumberInput = document.getElementById("card-number");
    const cvvInput = document.getElementById("cvv");
    const selectBox = document.querySelector(".select-box");
    const button = selectBox?.querySelector("button");
    const dropdown = selectBox?.querySelector(".dropdown-content");
    const toggle = selectBox?.querySelector(".toggle");

    // Kart numarası formatı
    if (cardNumberInput) {
        cardNumberInput.addEventListener("input", (e) => {
            const target = e.target;
            let value = target.value.replace(/\D/g, "");
            value = value.replace(/(.{4})/g, "$1 ").trim();
            target.value = value;
        });
    }

    // CVV sadece rakam
    if (cvvInput) {
        cvvInput.addEventListener("input", (e) => {
            const target = e.target;
            target.value = target.value.replace(/\D/g, "");
        });
    }

    // Form gönderimi
    // if (form) {
    //     form.addEventListener("submit", (e) => {
    //         e.preventDefault();
    //         const submitButton = form.querySelector('button[type="submit"]');
    //         if (submitButton) {
    //             submitButton.disabled = true;
    //             submitButton.innerHTML = `
    //                 <svg class="animate-spin h-5 w-5 text-white inline-block mr-2" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
    //                     <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
    //                     <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
    //                 </svg>
    //                 İşleminiz Gerçekleştiriliyor...
    //             `;

    //             // 2 saniye sonra başarılı sayfasına yönlendir
    //             setTimeout(() => {
    //                 window.location.href = "/reklamsiz/basarili";
    //             }, 2000);
    //         }
    //     });
    // }
};

// DOM yüklendiğinde ödeme formunu başlat
if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initPaymentForm);
} else {
    initPaymentForm();
}

// Yeni kullanıcı sayfası işlemleri
const initNewUserPage = () => {
    const planButtons = document.querySelectorAll(
        'button[class*="bg-red-600"], button[class*="border-red-600"]'
    );

    planButtons.forEach((button) => {
        button.addEventListener("click", () => {
            // Plan seçimine göre ödeme sayfasına yönlendir
            const planType = button
                .closest("div")
                .querySelector("h3")
                .textContent.toLowerCase();
            let planPrice = "";

            if (planType.includes("aylık")) {
                planPrice = "19.99";
            } else if (planType.includes("yıllık")) {
                planPrice = "199.99";
            } else if (planType.includes("öğrenci")) {
                planPrice = "9.99";
            }
        });
    });
};

// Layout menu işlemleri
const initLayoutMenus = () => {
    // User Menu Toggle
    const userButton = document.querySelector('[data-toggle="user-menu"]');
    const userDropdown = document.querySelector(".user-dropdown");

    if (userButton && userDropdown) {
        userButton.addEventListener("click", (e) => {
            e.stopPropagation();
            userDropdown.classList.toggle("show");
            userDropdown.classList.toggle("hidden");
        });

        // Close dropdown when clicking outside
        document.addEventListener("click", (e) => {
            if (
                !userDropdown.contains(e.target) &&
                !userButton.contains(e.target)
            ) {
                userDropdown.classList.remove("show");
                userDropdown.classList.add("hidden");
            }
        });
    }

    // Mobile menu toggle
    const mobileMenuButton = document.getElementById("mobile-menu-button");
    const mobileMenu = document.getElementById("mobile-menu");

    if (mobileMenuButton && mobileMenu) {
        mobileMenuButton.addEventListener("click", () => {
            const isExpanded =
                mobileMenuButton.getAttribute("aria-expanded") === "true";
            mobileMenuButton.setAttribute("aria-expanded", String(!isExpanded));
            mobileMenu.classList.toggle("hidden");
        });
    }
};

// DOM yüklendiğinde layout menülerini başlat
if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initLayoutMenus);
} else {
    initLayoutMenus();
}

// Accordion işlevselliği
document.querySelectorAll("[data-accordion-target]").forEach((button) => {
    button.addEventListener("click", () => {
        const targetId = button.getAttribute("data-accordion-target") || "";
        const content = document.getElementById(targetId);
        const icon = button.querySelector("svg");

        // Diğer tüm açık accordion'ları kapat
        document
            .querySelectorAll("[data-accordion-target]")
            .forEach((otherButton) => {
                if (otherButton !== button) {
                    const otherId =
                        otherButton.getAttribute("data-accordion-target") || "";
                    const otherContent = document.getElementById(otherId);
                    const otherIcon = otherButton.querySelector("svg");

                    otherContent?.classList.add("hidden");
                    otherIcon?.classList.remove("rotate-180");
                }
            });

        // Tıklanan accordion'ı aç/kapat
        content?.classList.toggle("hidden");
        icon?.classList.toggle("rotate-180");
    });
});

// Modal işlevselliği
const modal = document.getElementById("supportModal");
const supportButton = document.getElementById("supportButton");
const closeModal = document.getElementById("closeModal");
const closeButtons = document.querySelectorAll("button");

supportButton?.addEventListener("click", () => {
    modal?.classList.remove("hidden");
    document.body.style.overflow = "hidden";
});

const closeModalFunction = () => {
    modal?.classList.add("hidden");
    document.body.style.overflow = "";
};

closeModal?.addEventListener("click", closeModalFunction);

closeButtons.forEach((button) => {
    const buttonText = button.textContent?.trim() || "";
    if (buttonText === "Kapat") {
        button.addEventListener("click", closeModalFunction);
    }
});

// Modal dışına tıklandığında kapatma
modal?.addEventListener("click", (e) => {
    if (e.target === modal) {
        closeModalFunction();
    }
});

// ESC tuşu ile kapatma
document.addEventListener("keydown", (e) => {
    if (e.key === "Escape" && !modal?.classList.contains("hidden")) {
        closeModalFunction();
    }
});

// Form tipi değişimi
const faturaRadios = document.querySelectorAll('input[name="fatura_tipi"]');
const bireyselForm = document.getElementById("bireysel-form");
const kurumsalForm = document.getElementById("kurumsal-form");

faturaRadios.forEach((radio) => {
    radio.addEventListener("change", (e) => {
        const target = e.target;
        if (target.value === "bireysel") {
            bireyselForm?.classList.remove("hidden");
            kurumsalForm?.classList.add("hidden");
            // Kurumsal form alanlarını disabled yap
            toggleFormFields(kurumsalForm, true);
            toggleFormFields(bireyselForm, false);
        } else {
            bireyselForm?.classList.add("hidden");
            kurumsalForm?.classList.remove("hidden");
            // Bireysel form alanlarını disabled yap
            toggleFormFields(bireyselForm, true);
            toggleFormFields(kurumsalForm, false);
        }
    });
});

// Form alanlarını disable/enable yapma
function toggleFormFields(form, disabled) {
    if (!form) return;
    const inputs = form.querySelectorAll("input, select, textarea");
    inputs.forEach((input) => {
        if (
            input instanceof HTMLInputElement ||
            input instanceof HTMLSelectElement ||
            input instanceof HTMLTextAreaElement
        ) {
            input.disabled = disabled;
        }
    });
}

const floatingAds = document.getElementById("floating-ads");

if (floatingAds) {
    const defaultTop = 154; // top-44 = 44 * 4 = 176px
    const scrolledTop = 80; // top-20 = 20 * 4 = 80px
    const triggerPoint = 100;

    window.addEventListener("scroll", () => {
        if (window.scrollY > triggerPoint) {
            floatingAds.style.top = `${scrolledTop}px`;
        } else {
            floatingAds.style.top = `${defaultTop}px`;
        }
    });

}

function addSlideToSlider(splideInstance, htmlContent, index) {
    if (!splideInstance || typeof splideInstance.add !== "function") {
        return;
    }

    splideInstance.add(htmlContent, index);
}

// addSlideToSlider(slider, "<div>Google Reklam Kodu</div>", 5);
