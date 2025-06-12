// Initialize cart if it doesn't exist
if (!localStorage.getItem("cartItems")) {
  localStorage.setItem("cartItems", "[]");
}

function updateCartTotalUI() {
  try {
    const cartItems = JSON.parse(localStorage.getItem("cartItems") || "[]");
    const total = cartItems.reduce(
      (sum, item) => sum + (parseFloat(item.price) || 0),
      0
    );

    const cartTotalElement = document.getElementById("cartTotalPrice");
    if (cartTotalElement) {
      cartTotalElement.textContent = total.toLocaleString("vi-VN") + "đ";
    }
  } catch (error) {
    console.error("Error updating cart total:", error);
    // Reset cart if there's an error
    localStorage.setItem("cartItems", "[]");
    const cartTotalElement = document.getElementById("cartTotalPrice");
    if (cartTotalElement) {
      cartTotalElement.textContent = "0đ";
    }
  }
}

// Update cart total when page loads
document.addEventListener("DOMContentLoaded", updateCartTotalUI);

// Update cart total when storage changes (for multiple tabs)
window.addEventListener("storage", function (e) {
  if (e.key === "cartItems") {
    updateCartTotalUI();
  }
});
