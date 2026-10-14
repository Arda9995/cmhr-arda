/* https://github.com/DiemenDesign/summernote-image-attributes */
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery'], factory);
    } else if (typeof module === 'object' && module.exports) {
        module.exports = factory(require('jquery'));
    } else {
        factory(window.jQuery);
    }
}(function ($) {
    var readFileAsDataURL = function (file) {
        return $.Deferred(function (deferred) {
            $.extend(new FileReader(), {
                onload: function (e) {
                    var sDataURL = e.target.result;
                    deferred.resolve(sDataURL);
                },
                onerror: function () {
                    deferred.reject(this);
                }
            }).readAsDataURL(file);
        }).promise();
    };
    $.extend(true, $.summernote.lang, {
        'en-US': { /* US English(Default Language) */
            photoEditor: {
                src: 'Source',
            }
        }
    });
    $.extend($.summernote.options, {
        photoEditor: {
            icon: '<i class="fa-solid fa-pen-to-square"></i>',
            removeEmpty: true,
            disableUpload: false,
            imageFolder: ''
        }
    });
    $.extend($.summernote.plugins, {
        'photoEditor': function (context) {
            var self = this,
                ui = $.summernote.ui,
                $note = context.layoutInfo.note,
                $editor = context.layoutInfo.editor,
                $editable = context.layoutInfo.editable,
                options = context.options,
                lang = options.langInfo

            context.memo('button.photoEditor', function () {
                var button = ui.button({
                    contents: options.photoEditor.icon,
                    container: "body",
                    tooltip: lang.photoEditor.tooltip,
                    click: function () {
                        context.invoke('photoEditor.show');
                    }
                });
                return button.render();
            });
            this.initialize = function () {
                var $container = options.dialogsInBody ? $(document.body) : $editor;

                var body = '<iframe style="width:100%; height:90%;" scrolling="no" class="photoEditModal-iframe" frameborder="0"></iframe>';

                this.$dialog = ui.dialog({
                    title: "Görseli Düzenle",
                    body: body,
                    className: "note-photoEdit-modal"

                }).render().appendTo($container);
            };
            this.destroy = function () {
                ui.hideDialog(this.$dialog);
                this.$dialog.remove();
            };
            this.bindEnterKey = function ($input, $btn) {
                $input.on('keypress', function (e) {
                    if (e.keyCode === 13) $btn.trigger('click');
                });
            };

            this.show = function () {
                var $img = $($editable.data('target'));
                var imgInfo = {
                    imgDom: $img,
                    src: $img.attr('src'),
                    imgLink: $($img).parent().is("a") ? $($img).parent() : null
                };
                this.showPhotoEditorDialog(imgInfo).then(function (imgInfo) {
                    ui.hideDialog(self.$dialog);
                    var $img = imgInfo.imgDom;
                    if (options.photoEditor.removeEmpty) {
                        if (imgInfo.src) $img.attr('src', imgInfo.src); else $img.attr('src', '#');
                    }
                    $note.val(context.invoke('code'));
                    $note.change();
                });
            };
            this.showPhotoEditorDialog = function (imgInfo) {
                return $.Deferred(function (deferred) {

                    ui.onDialogShown(self.$dialog, function () {
                        // Set the value of the input field here
                        $('.photoEditModal-iframe').attr('src', '/News/PhotoEdit?imgSrc=' + imgInfo.src);

                        context.triggerEvent('dialog.shown');
                    });
                    ui.showDialog(self.$dialog);
                });
            };


        }
    });
}));