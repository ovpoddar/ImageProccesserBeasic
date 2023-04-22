window.onload = function () {
    const input = document.getElementById("image");
    const button = document.getElementById("submit");
    const output = document.getElementById("output");
    var c = document.createElement("canvas");
    var image = new Image();


    input.oninput = function (event) {
        image.src = URL.createObjectURL(event.target.files[0]);
        image.onload = function () {
            URL.revokeObjectURL(image.src)
        };
    };

    button.onclick = function () {
        if (input.src == null) {
            alert("file not found");
            return;
        }
        try {
            c.width = image.width;
            c.height = image.height;

            var ctx = c.getContext("2d");
            ctx.drawImage(image, 0, 0);

            var pixelArr = ctx.getImageData(0, 0, image.width, image.height).data;
            var table = maketable(pixelArr, image.height, image.width);
            output.appendChild(table);
        } catch {
            alert(error);
        }
    };

};



function maketable(data, height, width) {
    var table = document.createElement("table");
    for (let y = 0; y < height; y++) {
        var tr = document.createElement("tr");
        for (let x = 0; x < width; x++) {
            var td = document.createElement("td");
            let p = (x + (y * width)) * 4;
            var onePixel = makePixel(data[p], data[p + 1], data[p + 2], data[p + 3]);
            td.appendChild(onePixel);
            tr.appendChild(td);
        }

        table.appendChild(tr);
    }
    return table;
}

function makePixel(r, g, b, a) {
    var pixelBox = document.createElement("div");
    pixelBox.setAttribute("class", "pixelBox");
    pixelBox.innerHTML = `${r} ${g} ${b} ${a}`;
    pixelBox.style.backgroundColor = `rgba(${r}, ${g}, ${b}, ${a})`;
    return pixelBox;
}