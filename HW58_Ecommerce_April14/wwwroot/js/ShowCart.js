$(() => {
    $(".quantity").on('click', function () {
        const button = $(this);
        const scid = button.data('scid');
        const pid = button.data('pid');
        const quantity = button.data('quantity');
        $("#scid").val(scid);
        $("#pid").val(pid);
        $("#quantity").val(quantity);
        $("#edititem").modal();
    });

});