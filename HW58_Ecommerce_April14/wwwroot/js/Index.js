$(() => {
    const id = document.getElementById('catid').value;  
    $(".product").remove();
    $.post('/home/getproducts', { id }, function (products) {
        products.forEach(product => populateproducts(product));
    });
    $(".category").on('click', function () {
        const button = $(this);
        const id = button.data('id');
        $(".product").remove();
        $.post('/home/getproducts', { id }, function (products) {
            products.forEach(product => populateproducts(product));
        });
    });
    const populateproducts = product => {
        $(".product-page").append(`  <div class="row product" >
                    <div class="col-sm-4 col-lg-4 col-md-4">
                        <div class="thumbnail">
                            <img src="/UploadedFiles/${product.fileName}" alt="">
                            <div class="caption">
                                <h4 class="pull-right">${product.amount} </h4 >
                                <h4>
                                    <a href="/home/showproduct?id=${product.id}">${product.name}</a >
                                </h4>
                                
                            </div>                           
                        </div>
                    </div>
                </div>`);
    };
});