$(() => {


    $('#cart-btn').on('click', function () {

        const p = {
            productid: $(this).data('productid'),
            quantity: $('#quantity').val()
        };

        $.post('/home/addtocart', p, function () {

            $('#view-cart-modal').modal();

        });

    });

    $('#item').on('click', '.delete', function () {

        console.log('hello');
        const p = {
            productid: $(this).data('productid'),
            cartid: $(this).data('cartid')
        }

        $.post("/home/deletefromcart", p);

    });



})