global.$ = require("jquery");

require("bootstrap");
require("./components/footer.js");

(function ($) {
    $(document).ready(function () {
        footer.init();
    });
})(global.$);