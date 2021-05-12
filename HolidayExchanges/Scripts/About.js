$(document).ready(function () {
    let footerIcons = document.getElementById('FooterSocialIcons');
    footerIcons.classList.remove('d-sm-block');

    let footerText = document.getElementById('CopyrightText');
    footerText.classList.remove('offset-sm-1', 'col-sm-6', 'offset-lg-3', 'col-lg-3');

    document.getElementById('GitHubBtn').onclick = function () {
        location.href = "https://github.com/BrendanNguyenCS";
    };

    document.getElementById('DiscordBtn').onclick = function () {
        location.href = "https://www.discord.com";
    };

    document.getElementById('FBBtn').onclick = function () {
        location.href = "https://www.facebook.com";
    };
});