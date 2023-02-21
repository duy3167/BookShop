
    const imgHelper = {
        previewImage: function () {
            document.getElementById("img_preview").src = '/img/deco/demo-image.jpg';
            var imageReader = new FileReader();
            imageReader.readAsDataURL(document.getElementById("pro_img").files[0]);
            imageReader.onload = function (oFREvent) {
                document.getElementById("img_preview").src = oFREvent.target.result;
            };
        }
    }
