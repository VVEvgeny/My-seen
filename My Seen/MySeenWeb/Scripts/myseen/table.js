/*!
 * ��������� ������� ������ ������
 * 
 * ����� .table.table-striped
 */

+function ($) {
    $(function () {
        var $table = $('.table.table-striped');
        var $thead = $table.find('thead');
        var $ths = $thead.find('tr th');
        var offsetTop = $thead.offset().top;
        var thWidths = [];

        $ths.each(function (index, element) {
            var $th = $(element);

            thWidths.push($th.width());
        });

        $(window).on('scroll', function () {
            var scrollTop = $(window).scrollTop() + 50;

            if (scrollTop >= offsetTop) {
                $thead.css({ 'position': 'fixed', 'top': '50px' });

                var $firstTr = $table.find('tbody tr:first');

                $ths.each(function (index, element) {
                    var $th = $(element);

                    $th.width(thWidths[index]);
                    $firstTr.find('td').eq(index).width(thWidths[index]);
                });
            } else {
                $thead.css({ 'position': 'relative', 'top': 'auto' });
            }
        });
    });
}(jQuery);


