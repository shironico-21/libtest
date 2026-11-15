
const API_URL = "https://localhost:7186/api/data";
async function fetchData() {
    try {
        const response = await fetch(API_URL);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        document.getElementById('response_text').innerText = data.message;// Process the data as needed
    } catch (error) {
        console.error('Error fetching data:', error);
        document.getElementById('response_text').innerText = 'Error fetching data';
    }
}
fetchData();
document.addEventListener("click", function (e) {
    const btn = e.target.closest(".btnm");
    if (!btn) return; // click wasn't on a cart button

    const card = btn.closest(".card");

    const product = {
        title: card.querySelector(".title").innerText.trim(),
        desc: card.querySelector(".desc").innerText.trim(),
        image: card.querySelector("img").src
    };

    let cart = JSON.parse(localStorage.getItem("cart")) || [];

    const exists = cart.some(item => item.image === product.image);

    if (!exists) {
        cart.push(product);
        localStorage.setItem("cart", JSON.stringify(cart));
        alert("added to cart");
    } else {
        alert("already in cart");
    }
});
