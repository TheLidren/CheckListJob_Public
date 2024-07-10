$(document).ready(function () {
    $('.submit').click(function (e) {
        e.preventDefault();
        if (this.getAttribute('parameter') == '') {
            document.getElementById("countRows").value = this.getAttribute('data-countRow');
        }
        else document.getElementById("pageSort").value = this.getAttribute('parameter');
        document.getElementById('filter-form').submit();
    });
});