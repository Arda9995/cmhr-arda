$(document).ready(function () {
    tinymce.init({
        selector: 'textarea.content.tinymce',
        relative_urls: false,
        language: 'tr',
        entity_encoding: "raw",
        /*cleanup: false,                     // Otomatik temizlik aktif
        verify_html: true,                // HTML doğrulama
        remove_trailing_brs: true,        // Gereksiz <br> sil
        remove_empty: false,               // Boş <p>, <div> vs. sil
        forced_root_block: 'p',           // Boş bloklar yerine <p> kullan
        force_br_newlines: false,         // <br> kullanımını azalt
        force_p_newlines: true,           // Satırları <p> ile sar
        browser_spellcheck: true,
        remove_linebreaks: false,*/
        statusbar: true,
        quickbars_insert_toolbar: false,
        plugins: 'preview importcss searchreplace autolink autosave save directionality code visualblocks visualchars fullscreen image imagetools link media template codesample table charmap pagebreak nonbreaking anchor insertdatetime advlist lists wordcount help quickbars emoticons',
        imagetools_cors_hosts: ['media.cumhuriyet.com.tr', 'cmhmedia.b-cdn.net'],
        menubar: 'file edit view insert format tools table help',
        toolbar: ['restoredraft | undo redo | bold italic underline strikethrough | fontselect fontsizeselect formatselect | alignleft aligncenter alignright alignjustify | outdent indent |  numlist bullist',
            'forecolor backcolor removeformat cleanup uppercase lowercase | pagebreak | blockquote charmap emoticons | twitterButton youtubeButton | mediaGalleryButton media imagetools template link anchor | preview fullscreen save print code',],
        //toolbar_sticky: true,
        extended_valid_elements: 'script[async|src|charset|type]', // script etiketi için özel izin
        autosave_ask_before_unload: true,
        autosave_interval: "60s",
        autosave_prefix: "{path}{query}-{id}-",
        autosave_restore_when_empty: false,
        autosave_retention: "60m",
        image_advtab: true,
        content_css: 'https://www.tiny.cloud/css/codepen.min.css',
        
        importcss_append: true,
        setup: function (editor) {
            const updateAnchorRelAttributes = () => {
                const anchors = editor.getDoc().querySelectorAll('a');

                anchors.forEach(a => {
                    const href = a.getAttribute('href');
                    const rel = a.getAttribute('rel') || '';

                    // Cumhuriyet içi linkler için dokunma
                    if (!href || href.includes('cumhuriyet.com.tr')) return;

                    // rel yoksa veya dofollow içermiyorsa müdahale et
                    const relLower = rel.toLowerCase();
                    const hasDofollow = relLower.includes('dofollow');

                    if (!hasDofollow) {
                        a.setAttribute('rel', 'nofollow noopener');
                    }
                });
            };

            editor.on('SetContent NodeChange', updateAnchorRelAttributes);

            editor.ui.registry.addIcon('youtube', `
  <svg width="24" height="24" viewBox="0 0 24 24">
    <path fill="currentColor" d="M23.498 6.186a2.974 2.974 0 00-2.095-2.105C19.504 3.5 12 3.5 12 3.5s-7.504 0-9.403.581A2.974 2.974 0 00.502 6.186 31.739 31.739 0 000 12a31.739 31.739 0 00.502 5.814 2.974 2.974 0 002.095 2.105C4.496 20.5 12 20.5 12 20.5s7.504 0 9.403-.581a2.974 2.974 0 002.095-2.105A31.739 31.739 0 0024 12a31.739 31.739 0 00-.502-5.814zM9.75 15.02V8.98L15.5 12l-5.75 3.02z"/>
  </svg>
`);

            editor.ui.registry.addButton('youtubeButton', {
                icon: 'youtube', // varsayılan ikon yerine özel YouTube SVG tanımı da ekleyebilirsin
                tooltip: 'YouTube Video Ekle',
                onAction: function () {
                    const videoId = prompt("YouTube video ID'sini girin (örn: dQw4w9WgXcQ):");

                    if (videoId && /^[a-zA-Z0-9_-]{11}$/.test(videoId)) {
                        const embedHtml = `
<p>
  <div class="responsive-container">
    <iframe src="https://www.youtube.com/embed/${videoId}" 
            frameborder="0" 
            allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" 
            allowfullscreen>
    </iframe>
  </div>
</p>`;

                        editor.insertContent(embedHtml);
                    } else {
                        alert("Geçerli bir YouTube video ID'si girilmelidir.");
                    }
                }
            });

  
            editor.ui.registry.addButton('uppercase', {
                text: 'a→A',
                tooltip: 'Küçük Harften Büyük Harfe Çevir',
                onAction: function () {
                    const content = editor.selection.getContent({ format: 'text' });
                    editor.selection.setContent(content.toLocaleUpperCase('tr-TR'));
                }
            });

            editor.ui.registry.addButton('lowercase', {
                text: 'A→a',
                tooltip: 'Büyük Harften Küçük Harfe Çevir',
                onAction: function () {
                    const content = editor.selection.getContent({ format: 'text' });
                    editor.selection.setContent(content.toLocaleLowerCase('tr-TR'));
                }
            });
            // Boş etiketleri temizle
            editor.ui.registry.addButton('cleanup', {
                text: '🧹 Boşluk Temizle',
                tooltip: 'Boş etiketleri temizle',
                onAction: function () {
                    let content = editor.getContent();

                    // <p></p>, <div></div> gibi boş tag'leri temizle
                    content = content.replace(/<p>(\s|&nbsp;)*<\/p>/gi, '');
                    content = content.replace(/<div>(\s|&nbsp;)*<\/div>/gi, '');
                    content = content.replace(/<span>(\s|&nbsp;)*<\/span>/gi, '');
                    content = content.replace(/<strong>(\s|&nbsp;)*<\/strong>/gi, '');
                    content = content.replace(/<em>(\s|&nbsp;)*<\/em>/gi, '');
                    content = content.replace(/<b>(\s|&nbsp;)*<\/b>/gi, '');
                    content = content.replace(/<i>(\s|&nbsp;)*<\/i>/gi, '');

                    editor.setContent(content);
                }
            });
            editor.ui.registry.addButton('mediaGalleryButton', {
                icon: 'image', // TinyMCE'nin yerleşik resim ikonu
                tooltip: 'Medya Galeriden Görsel Ekle',
                onAction: function () {
                    const img_id = "tinymce-photogallery";
                    const modalBody = $('#media_gallery_modal').find('.modal-body');
                    modalBody.attr('id', img_id);
                    $('#media_gallery_modal').modal('show');
                }
            });

        },
        height: 600,
        image_caption: true,
        quickbars_selection_toolbar: 'blocks | bold italic underline uppercase lowercase | alignleft aligncenter alignright alignjustify || quicklink blockquote imagetools',
        noneditable_noneditable_class: "mceNonEditable",
        //toolbar_mode: 'sliding',
        contextmenu: "locks | bold italic underline | quicklink blockquote link mediaGalleryButton imagetools table",
    });
});