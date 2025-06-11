function updateCartTotalUI() {
  const cartItems = JSON.parse(localStorage.getItem("cartItems") || "[]");
  const total = cartItems.reduce(
    (sum, item) => sum + (parseFloat(item.price) || 0),
    0
  );

  const cartTotalElement = document.getElementById("cartTotalPrice");
  if (cartTotalElement) {
    cartTotalElement.textContent = total.toLocaleString("vi-VN") + "đ";
  }
}

document.addEventListener("DOMContentLoaded", updateCartTotalUI);
