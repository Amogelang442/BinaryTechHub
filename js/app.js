document.addEventListener("DOMContentLoaded", () => {
  // 1. Initialize EmailJS safely
  if (typeof emailjs !== "undefined") {
    emailjs.init("wZ6cNuBQiu2pEciVU");
  } else {
    console.warn("EmailJS SDK script tag missing from index.html head/body.");
  }

  const EMAILJS_SERVICE_ID = "service_5eysruj";
  const EMAILJS_RECEIPT_TEMPLATE_ID = "template_4orua3g"; 
  const EMAILJS_OTP_TEMPLATE_ID = "template_i7k3bpw"; 
  const API_BASE_URL = "http://localhost:5185/api";

  // 2. DOM Elements
  const categoryContainer = document.getElementById("category-buttons");
  const productContainer = document.getElementById("product-grid");
  const cartToggleBtn = document.getElementById("cart-toggle-btn");
  const closeCartBtn = document.getElementById("close-cart-btn");
  const cartDrawer = document.getElementById("cart-drawer");
  const cartOverlay = document.getElementById("cart-overlay");
  const cartItemsContainer = document.getElementById("cart-items");
  const cartCountElem = document.getElementById("cart-count");
  const subtotalElem = document.getElementById("cart-subtotal");
  const vatElem = document.getElementById("cart-vat");
  const totalElem = document.getElementById("cart-total");
  const checkoutBtn = document.getElementById("checkout-btn");

  // Checkout Modal Elements
  const checkoutOverlay = document.getElementById("checkout-overlay");
  const closeCheckoutBtn = document.getElementById("close-checkout-btn");
  const modalTotalPay = document.getElementById("modal-total-pay");
  const checkoutView = document.getElementById("checkout-view");
  const processingView = document.getElementById("processing-view");
  const successView = document.getElementById("success-view");
  const cardNumInput = document.getElementById("card-num");
  const cardBrand = document.getElementById("card-brand");

  let activePaymentMethod = "card";
  let currentActiveCategoryId = "all";

  // 3. Cart State Management
  let cart = JSON.parse(localStorage.getItem("bth_cart")) || [];

  function saveCart() {
    localStorage.setItem("bth_cart", JSON.stringify(cart));
    updateCartDisplay();
  }

  // 4. Render Categories & Catalog
  function renderCategories() {
    if (!categoryContainer || typeof categories === "undefined") return;
    let html = `<button class="filter-btn active" onclick="filterCatalog('all', this)">All</button>`;
    categories.forEach(cat => {
      html += `<button class="filter-btn" onclick="filterCatalog(${cat.id}, this)">${cat.name}</button>`;
    });
    categoryContainer.innerHTML = html;
  }

  window.filterCatalog = function(categoryId, btnElement) {
    if (!productContainer || typeof products === "undefined") return;

    currentActiveCategoryId = categoryId;

    if (categoryContainer) {
      const buttons = categoryContainer.querySelectorAll(".filter-btn");
      buttons.forEach(btn => btn.classList.remove("active"));
      if (btnElement) {
        btnElement.classList.add("active");
      }
    }

    const filtered = categoryId === "all" 
      ? products 
      : products.filter(p => p.categoryId === categoryId);

    productContainer.innerHTML = filtered.map(item => `
      <div class="product-card" data-id="${item.id}">
        <img src="${item.image}" alt="${item.name}" onerror="this.src='https://via.placeholder.com/300x200?text=No+Image'">
        <div class="product-info">
          <span class="product-category">${item.categoryName}</span>
          <h3 class="product-title">${item.name}</h3>
          <p class="product-specs">${item.specs}</p>
          <div class="product-footer">
            <span class="product-price">R ${Number(item.price).toFixed(2)}</span>
            <button class="add-btn" onclick="addToCart('${item.id}')" ${item.stock <= 0 ? 'disabled style="opacity:0.5;cursor:not-allowed;"' : ''}>
              ${item.stock <= 0 ? 'Out of Stock' : 'Add to Cart'}
            </button>
          </div>
        </div>
      </div>
    `).join("");
  };

  // Event listener: Automatically refresh cards when API finishes fetching from MySQL
  window.addEventListener('bth:products-loaded', () => {
    const activeBtn = categoryContainer ? categoryContainer.querySelector(`.filter-btn.active`) : null;
    window.filterCatalog(currentActiveCategoryId, activeBtn);
  });

  // 5. Cart Actions
  window.addToCart = function(productId) {
    const product = products.find(p => p.id === productId);
    if (!product) return;

    const existingIndex = cart.findIndex(item => item.id === productId);
    if (existingIndex > -1) {
      cart[existingIndex].quantity += 1;
    } else {
      cart.push({
        id: product.id,
        name: product.name,
        price: product.price,
        image: product.image,
        quantity: 1
      });
    }

    saveCart();
    openCart();
  };

  window.updateQty = function(productId, delta) {
    const item = cart.find(i => i.id === productId);
    if (!item) return;

    item.quantity += delta;
    if (item.quantity <= 0) {
      cart = cart.filter(i => i.id !== productId);
    }
    saveCart();
  };

  window.removeFromCart = function(productId) {
    cart = cart.filter(i => i.id !== productId);
    saveCart();
  };

  function updateCartDisplay() {
    const totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
    if (cartCountElem) cartCountElem.textContent = totalItems;

    if (cart.length === 0) {
      if (cartItemsContainer) cartItemsContainer.innerHTML = `<p class="cart-empty-msg">Your shopping cart is empty.</p>`;
      if (subtotalElem) subtotalElem.textContent = "R 0.00";
      if (vatElem) vatElem.textContent = "R 0.00";
      if (totalElem) totalElem.textContent = "R 0.00";
      if (checkoutBtn) checkoutBtn.disabled = true;
      return;
    }

    if (checkoutBtn) checkoutBtn.disabled = false;

    if (cartItemsContainer) {
      cartItemsContainer.innerHTML = cart.map(item => `
        <div class="cart-item">
          <img src="${item.image}" alt="${item.name}" onerror="this.src='https://via.placeholder.com/60'">
          <div class="cart-item-details">
            <div class="cart-item-title">${item.name}</div>
            <div class="cart-item-price">R ${(item.price * item.quantity).toFixed(2)}</div>
            <div class="cart-qty-controls">
              <button class="qty-btn" onclick="updateQty('${item.id}', -1)">-</button>
              <span class="qty-count">${item.quantity}</span>
              <button class="qty-btn" onclick="updateQty('${item.id}', 1)">+</button>
              <button class="remove-item-btn" onclick="removeFromCart('${item.id}')">Remove</button>
            </div>
          </div>
        </div>
      `).join("");
    }

    const subtotal = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    const vat = subtotal * 0.15;
    const total = subtotal + vat;

    if (subtotalElem) subtotalElem.textContent = `R ${subtotal.toFixed(2)}`;
    if (vatElem) vatElem.textContent = `R ${vat.toFixed(2)}`;
    if (totalElem) totalElem.textContent = `R ${total.toFixed(2)}`;
  }

  // 6. Drawer Navigation
  function openCart() {
    if (cartDrawer) cartDrawer.classList.add("open");
    if (cartOverlay) cartOverlay.classList.add("active");
  }

  function closeCart() {
    if (cartDrawer) cartDrawer.classList.remove("open");
    if (cartOverlay) cartOverlay.classList.remove("active");
  }

  if (cartToggleBtn) cartToggleBtn.addEventListener("click", openCart);
  if (closeCartBtn) closeCartBtn.addEventListener("click", closeCart);
  if (cartOverlay) cartOverlay.addEventListener("click", closeCart);

  // 7. Checkout Modal Open/Close Logic
  function openCheckout() {
    if (cart.length === 0) return;
    closeCart();

    const subtotal = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    const total = subtotal * 1.15;
    if (modalTotalPay) modalTotalPay.textContent = `R ${total.toFixed(2)}`;

    if (checkoutView) checkoutView.style.display = "block";
    if (processingView) processingView.style.display = "none";
    if (successView) successView.style.display = "none";
    if (checkoutOverlay) checkoutOverlay.classList.add("active");
  }

  function closeCheckoutModal() {
    if (checkoutOverlay) checkoutOverlay.classList.remove("active");
  }

  if (checkoutBtn) checkoutBtn.addEventListener("click", openCheckout);
  if (closeCheckoutBtn) closeCheckoutBtn.addEventListener("click", closeCheckoutModal);

  // 8. Payment Method Selection
  window.selectPayMethod = function(method) {
    activePaymentMethod = method;
    const tabs = document.querySelectorAll(".payment-tabs .tab-btn");
    tabs.forEach(t => t.classList.remove("active"));
    if (event && event.target) event.target.classList.add("active");

    const cardFields = document.getElementById("card-fields");
    const eftFields = document.getElementById("eft-fields");

    if (method === "card") {
      if (cardFields) cardFields.style.display = "block";
      if (eftFields) eftFields.style.display = "none";
      if (cardNumInput) cardNumInput.required = true;
    } else {
      if (cardFields) cardFields.style.display = "none";
      if (eftFields) eftFields.style.display = "block";
      if (cardNumInput) cardNumInput.required = false;
    }
  };

  // 9. Card Format & Detection
  if (cardNumInput) {
    cardNumInput.addEventListener("input", (e) => {
      let val = e.target.value.replace(/\D/g, "");
      if (cardBrand) {
        if (val.startsWith("4")) {
          cardBrand.textContent = "VISA";
          cardBrand.style.color = "#1d4ed8";
        } else if (val.startsWith("5") || val.startsWith("2")) {
          cardBrand.textContent = "MASTERCARD";
          cardBrand.style.color = "#ea580c";
        } else {
          cardBrand.textContent = "CARD";
          cardBrand.style.color = "#475569";
        }
      }
      e.target.value = val.match(/.{1,4}/g)?.join(" ") || "";
    });
  }

  let generatedOtp = null;
  let pendingOrderData = null;

  // Phase 1: User clicks "Pay Now" -> Generate OTP & dispatch email
  window.handlePaymentSubmit = function(e) {
    e.preventDefault();

    const customerName = document.getElementById("cust-name")?.value || "Valued Customer";
    const customerEmail = document.getElementById("cust-email")?.value || "";
    const customerAddress = document.getElementById("cust-address")?.value || "N/A";
    const subtotal = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    const totalAmount = (subtotal * 1.15).toFixed(2);
    const orderRef = "BTH-" + Math.floor(100000 + Math.random() * 900000);

    // Generate 6-digit numeric PIN
    generatedOtp = Math.floor(100000 + Math.random() * 900000).toString();

    // Cache checkout data until OTP verification
    pendingOrderData = {
      orderRef,
      customerName,
      customerEmail,
      customerAddress,
      totalAmount,
      cartSnapshot: [...cart]
    };

    if (checkoutView) checkoutView.style.display = "none";
    const otpView = document.getElementById("otp-view");
    const otpTarget = document.getElementById("otp-target-email");
    if (otpTarget) otpTarget.textContent = customerEmail;
    if (otpView) otpView.style.display = "block";

    // Send OTP via EmailJS
    emailjs.send(EMAILJS_SERVICE_ID, EMAILJS_OTP_TEMPLATE_ID, {
      customer_name: customerName,
      customer_email: customerEmail,
      total_amount: `R ${totalAmount}`,
      otp_code: generatedOtp
    })
    .then(() => console.log("OTP dispatched to email."))
    .catch(err => console.error("OTP send failed:", err));
  };

  // Phase 2: User enters the code & clicks "Confirm & Pay"
  window.verifyOtpAndPay = async function() {
    const enteredOtp = document.getElementById("otp-input").value.trim();
    const otpError = document.getElementById("otp-error");

    if (enteredOtp !== generatedOtp) {
      if (otpError) otpError.style.display = "block";
      return;
    }

    if (otpError) otpError.style.display = "none";

    const otpView = document.getElementById("otp-view");
    if (otpView) otpView.style.display = "none";
    if (processingView) processingView.style.display = "block";

    const { orderRef, customerName, customerEmail, customerAddress, totalAmount, cartSnapshot } = pendingOrderData;
    const itemsList = cartSnapshot.map(i => `- ${i.name} x${i.quantity} (R ${(i.price * i.quantity).toFixed(2)})`).join("\n");

    // 1. Post order to ASP.NET Core API to write into MySQL
    try {
      const orderPayload = {
        orderRef: orderRef,
        customerName: customerName,
        customerEmail: customerEmail,
        deliveryAddress: customerAddress,
        paymentMethod: activePaymentMethod === "card" ? "Credit/Debit Card (3D Secure)" : "Instant EFT",
        totalAmount: parseFloat(totalAmount),
        items: cartSnapshot.map(i => ({
          productId: i.id,
          quantity: i.quantity,
          unitPrice: i.price
        }))
      };

      const apiResponse = await fetch(`${API_BASE_URL}/orders`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(orderPayload)
      });

      if (!apiResponse.ok) {
        console.warn("Backend order persistence responded with error:", apiResponse.status);
      } else {
        console.log("Order stored successfully in MySQL database!");
      }
    } catch (apiErr) {
      console.warn("Could not post order to backend API:", apiErr);
    }

    // 2. Dispatch final EmailJS receipt
    const emailParams = {
      order_ref: orderRef,
      customer_name: customerName,
      customer_email: customerEmail,
      delivery_address: customerAddress,
      payment_method: activePaymentMethod === "card" ? "Credit/Debit Card (3D Secure)" : "Instant EFT",
      total_amount: `R ${totalAmount}`,
      items_list: itemsList
    };

    if (typeof emailjs !== "undefined") {
      emailjs.send(EMAILJS_SERVICE_ID, EMAILJS_RECEIPT_TEMPLATE_ID, emailParams)
        .then(() => console.log("Final receipt sent!"))
        .catch(err => console.error("Email send failed:", err))
        .finally(() => showReceipt(orderRef, customerName, totalAmount));
    } else {
      setTimeout(() => showReceipt(orderRef, customerName, totalAmount), 1000);
    }
  };

  function showReceipt(orderRef, customerName, totalAmount) {
    if (processingView) processingView.style.display = "none";
    if (successView) successView.style.display = "block";

    const refElem = document.getElementById("receipt-ref");
    const nameElem = document.getElementById("receipt-name");
    const amountElem = document.getElementById("receipt-amount");

    if (refElem) refElem.textContent = orderRef;
    if (nameElem) nameElem.textContent = customerName;
    if (amountElem) amountElem.textContent = `R ${totalAmount}`;

    cart = [];
    saveCart();
  }

  window.dismissCheckout = function() {
    closeCheckoutModal();
  };

  // Initialize
  renderCategories();
  filterCatalog("all");
  updateCartDisplay();
});