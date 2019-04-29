$(() => {
    $(".addtocart").on('click', function () {
        const button = $(this);
        const productid = button.data('id');
        const quantity = $("#quantity option:selected").val();
        $.post('/home/additemtocart', { productid, quantity }, function (shoppingcatid) { });
        $("#additem").modal();
    });

});