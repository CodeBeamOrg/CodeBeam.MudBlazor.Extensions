window.CodeBeamCode = {

    highlight: function (element) {
        if (window.Prism) {
            Prism.highlightElement(element);
        }
    },

    copy: function (text) {
        navigator.clipboard.writeText(text);
    },

    debounce: function (func, delay) {
        let timer;
        return function () {
            clearTimeout(timer);
            timer = setTimeout(() => func.apply(this, arguments), delay);
        };
    }
};
