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
  const API_BASE_URL = "https://binarytechhub-api.onrender.com/";

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

  // Customizer Modal Elements
  const customizerOverlay = document.getElementById("customizer-overlay");

  let activePaymentMethod = "card";
  let selectedBankName = "Capitec Bank";
  let currentActiveCategoryId = "all";

  // Customizer State
  let activeCustomizerProduct = null;
  let selectedColor = null;
  let selectedStorage = null;

  // 3. Cart State Management
  let cart = JSON.parse(localStorage.getItem("bth_cart")) || [];

  function saveCart() {
    localStorage.setItem("bth_cart", JSON.stringify(cart));
    updateCartDisplay();
  }

  // Helper to render product card HTML
  function createProductCardHtml(item) {
    const isOutOfStock = item.stock <= 0;
    const hasVariants = (item.colors && item.colors.length > 0) || (item.storages && item.storages.length > 0);
    const buttonText = isOutOfStock ? 'Out of Stock' : (hasVariants ? 'Customize & Buy' : 'Add to Cart');

    return `
      <div class="product-card" data-id="${item.id}">
        <img src="${item.image}" alt="${item.name}" onerror="this.src='https://via.placeholder.com/300x200?text=No+Image'">
        <div class="product-info">
          <span class="product-category">${item.categoryName}</span>
          <h3 class="product-title">${item.name}</h3>
          <p class="product-specs">${item.specs}</p>
          <div class="product-footer">
            <span class="product-price">R ${Number(item.price).toFixed(2)}</span>
            <button class="add-btn" onclick="addToCart('${item.id}')" ${isOutOfStock ? 'disabled style="opacity:0.5;cursor:not-allowed;"' : ''}>
              ${buttonText}
            </button>
          </div>
        </div>
      </div>
    `;
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

    // Reset search input value if category button is clicked
    const searchInput = document.getElementById("catalog-search-input");
    if (searchInput && btnElement) {
      searchInput.value = "";
    }

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

    productContainer.innerHTML = filtered.map(createProductCardHtml).join("");
  };

  // Live Catalog Search Filter
  window.handleCatalogSearch = function(query) {
    if (!productContainer || typeof products === "undefined") return;

    const searchTerm = query.toLowerCase().trim();

    if (!searchTerm) {
      window.filterCatalog(currentActiveCategoryId);
      return;
    }

    const matches = products.filter(p => {
      const nameMatch = p.name.toLowerCase().includes(searchTerm);
      const specsMatch = (p.specs || "").toLowerCase().includes(searchTerm);
      const catMatch = (p.categoryName || "").toLowerCase().includes(searchTerm);
      return nameMatch || specsMatch || catMatch;
    });

    if (matches.length === 0) {
      productContainer.innerHTML = `
        <div style="grid-column: 1 / -1; text-align: center; padding: 48px 20px; color: var(--muted, #64748b);">
          <p style="font-size: 18px; font-weight: 700; color: var(--dark, #0f172a);">No products found matching "${query}"</p>
          <p style="font-size: 14px; margin-top: 6px;">Try searching for a different brand, device name, or storage specification.</p>
        </div>
      `;
      return;
    }

    productContainer.innerHTML = matches.map(createProductCardHtml).join("");
  };

  // Event listener: Automatically refresh cards when API finishes fetching from MySQL
  window.addEventListener('bth:products-loaded', () => {
    const activeBtn = categoryContainer ? categoryContainer.querySelector(`.filter-btn.active`) : null;
    window.filterCatalog(currentActiveCategoryId, activeBtn);
    updateCartDisplay();
  });

  // 5. Product Customizer Actions
  window.openCustomizer = function(product) {
    activeCustomizerProduct = product;
    selectedColor = product.colors && product.colors.length > 0 ? product.colors[0] : null;
    selectedStorage = product.storages && product.storages.length > 0 ? product.storages[0] : null;

    const imgElem = document.getElementById("cust-modal-img");
    const catElem = document.getElementById("cust-modal-category");
    const titleElem = document.getElementById("cust-modal-title");
    const priceElem = document.getElementById("cust-modal-price");
    const specsElem = document.getElementById("cust-modal-specs");

    if (imgElem) imgElem.src = product.image;
    if (catElem) catElem.textContent = product.categoryName;
    if (titleElem) titleElem.textContent = product.name;
    if (priceElem) priceElem.textContent = `R ${Number(product.price).toFixed(2)}`;
    if (specsElem) specsElem.textContent = product.specs;

    // Render Color Options
    const colorGroup = document.getElementById("cust-color-group");
    const colorContainer = document.getElementById("cust-color-options");
    if (product.colors && product.colors.length > 0) {
      if (colorGroup) colorGroup.style.display = "block";
      if (colorContainer) {
        colorContainer.innerHTML = product.colors.map((c, i) => `
          <button type="button" class="variant-pill ${i === 0 ? 'active' : ''}" onclick="pickColor(this, '${c}')">${c}</button>
        `).join("");
      }
    } else if (colorGroup) {
      colorGroup.style.display = "none";
    }

    // Render Storage Options
    const storageGroup = document.getElementById("cust-storage-group");
    const storageContainer = document.getElementById("cust-storage-options");
    if (product.storages && product.storages.length > 0) {
      if (storageGroup) storageGroup.style.display = "block";
      if (storageContainer) {
        storageContainer.innerHTML = product.storages.map((s, i) => `
          <button type="button" class="variant-pill ${i === 0 ? 'active' : ''}" onclick="pickStorage(this, '${s}')">${s}</button>
        `).join("");
      }
    } else if (storageGroup) {
      storageGroup.style.display = "none";
    }

    if (customizerOverlay) customizerOverlay.classList.add("active");
  };

  window.closeCustomizer = function() {
    if (customizerOverlay) customizerOverlay.classList.remove("active");
  };

  window.pickColor = function(btn, color) {
    selectedColor = color;
    if (btn && btn.parentElement) {
      btn.parentElement.querySelectorAll(".variant-pill").forEach(p => p.classList.remove("active"));
      btn.classList.add("active");
    }
  };

  window.pickStorage = function(btn, storage) {
    selectedStorage = storage;
    if (btn && btn.parentElement) {
      btn.parentElement.querySelectorAll(".variant-pill").forEach(p => p.classList.remove("active"));
      btn.classList.add("active");
    }
  };

  window.confirmCustomizedAddToCart = function() {
    if (!activeCustomizerProduct) return;
    executeAddToCart(activeCustomizerProduct, selectedColor, selectedStorage);
    window.closeCustomizer();
  };

  // 6. Cart Actions
  window.addToCart = function(productId) {
    const product = typeof products !== "undefined" ? products.find(p => p.id === productId) : null;
    if (!product) return;

    const hasColors = product.colors && product.colors.length > 0;
    const hasStorages = product.storages && product.storages.length > 0;

    // Open customizer modal if variants exist
    if (hasColors || hasStorages) {
      window.openCustomizer(product);
    } else {
      executeAddToCart(product, null, null);
    }
  };

  function executeAddToCart(product, color, storage) {
    let variantLabel = "";
    if (color && storage) variantLabel = ` (${color}, ${storage})`;
    else if (color) variantLabel = ` (${color})`;
    else if (storage) variantLabel = ` (${storage})`;

    const displayName = `${product.name}${variantLabel}`;

    // Total existing units of this base product already in cart
    const currentTotalInCart = cart
      .filter(item => item.id === product.id)
      .reduce((sum, item) => sum + item.quantity, 0);

    if (currentTotalInCart >= product.stock) {
      openCart();
      return;
    }

    // Match exact item by both ID and customized options
    const existingIndex = cart.findIndex(item => item.id === product.id && item.name === displayName);

    if (existingIndex > -1) {
      cart[existingIndex].quantity += 1;
    } else {
      if (product.stock <= 0) return;
      cart.push({
        id: product.id,       // Preserves exact MySQL primary key
        name: displayName,    // Displays variant label in cart & receipt
        price: product.price,
        image: product.image,
        quantity: 1
      });
    }

    saveCart();
    openCart();
  }

  window.updateQty = function(productId, delta, itemName) {
    const item = cart.find(i => i.id === productId && (!itemName || i.name === itemName));
    if (!item) return;

    if (delta > 0 && typeof products !== "undefined") {
      const catalogItem = products.find(p => p.id === productId);
      const currentTotalInCart = cart
        .filter(i => i.id === productId)
        .reduce((sum, i) => sum + i.quantity, 0);

      if (catalogItem && currentTotalInCart >= catalogItem.stock) {
        return;
      }
    }

    item.quantity += delta;
    if (item.quantity <= 0) {
      cart = cart.filter(i => !(i.id === productId && i.name === item.name));
    }
    saveCart();
  };

  window.removeFromCart = function(productId, itemName) {
    cart = cart.filter(i => !(i.id === productId && (!itemName || i.name === itemName)));
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
      if (checkoutBtn) {
        checkoutBtn.disabled = true;
        checkoutBtn.removeAttribute("title");
      }
      return;
    }

    let hasStockConflict = false;

    if (cartItemsContainer) {
      cartItemsContainer.innerHTML = cart.map(item => {
        const catalogItem = (typeof products !== "undefined") ? products.find(p => p.id === item.id) : null;
        const availableStock = catalogItem ? catalogItem.stock : 999;
        
        const totalBaseInCart = cart
          .filter(i => i.id === item.id)
          .reduce((sum, i) => sum + i.quantity, 0);

        const isAtMax = totalBaseInCart >= availableStock;

        if (totalBaseInCart > availableStock) {
          hasStockConflict = true;
        }

        // Escape single quotes for inline JS calls
        const escapedName = item.name.replace(/'/g, "\\'");

        return `
          <div class="cart-item">
            <img src="${item.image}" alt="${item.name}" onerror="this.src='https://via.placeholder.com/60'">
            <div class="cart-item-details">
              <div class="cart-item-title">${item.name}</div>
              <div class="cart-item-price">R ${(item.price * item.quantity).toFixed(2)}</div>
              <div class="cart-qty-controls">
                <button class="qty-btn" onclick="updateQty('${item.id}', -1, '${escapedName}')">-</button>
                <span class="qty-count">${item.quantity}</span>
                <button 
                  class="qty-btn" 
                  onclick="updateQty('${item.id}', 1, '${escapedName}')" 
                  ${isAtMax ? 'disabled title="Maximum available stock reached" style="opacity:0.35;cursor:not-allowed;"' : ''}
                >+</button>
                <button class="remove-item-btn" onclick="removeFromCart('${item.id}', '${escapedName}')">Remove</button>
              </div>
              ${isAtMax ? `
                <div style="font-size: 11px; color: #b45309; background: #fffbeb; padding: 3px 6px; border-radius: 4px; margin-top: 6px; font-weight: 600; display: inline-block; border: 1px solid #fde68a;">
                  ⚠️ Max stock reached (${availableStock} available)
                </div>
              ` : ''}
            </div>
          </div>
        `;
      }).join("");
    }

    if (checkoutBtn) {
      checkoutBtn.disabled = hasStockConflict;
      if (hasStockConflict) {
        checkoutBtn.title = "Please adjust cart quantities before checkout";
        checkoutBtn.style.opacity = "0.5";
        checkoutBtn.style.cursor = "not-allowed";
      } else {
        checkoutBtn.removeAttribute("title");
        checkoutBtn.style.opacity = "1";
        checkoutBtn.style.cursor = "pointer";
      }
    }

    const subtotal = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    const vat = subtotal * 0.15;
    const total = subtotal + vat;

    if (subtotalElem) subtotalElem.textContent = `R ${subtotal.toFixed(2)}`;
    if (vatElem) vatElem.textContent = `R ${vat.toFixed(2)}`;
    if (totalElem) totalElem.textContent = `R ${total.toFixed(2)}`;
  }

  // 7. Drawer Navigation
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

  // 8. Checkout Modal Open/Close Logic
  function openCheckout() {
    if (cart.length === 0) return;

    if (typeof products !== "undefined") {
      for (const item of cart) {
        const catalogItem = products.find(p => p.id === item.id);
        const totalBaseInCart = cart
          .filter(i => i.id === item.id)
          .reduce((sum, i) => sum + i.quantity, 0);

        if (catalogItem && totalBaseInCart > catalogItem.stock) {
          alert(`Cannot proceed: You have ${totalBaseInCart} units of "${catalogItem.name}" in your cart, but only ${catalogItem.stock} are available.`);
          return;
        }
      }
    }

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

  // 9. Payment Method & Bank Selection
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

  window.selectBank = function(btn, bankName) {
    selectedBankName = bankName;
    const hiddenInput = document.getElementById("selected-bank");
    if (hiddenInput) hiddenInput.value = bankName;

    const cards = document.querySelectorAll(".bank-card");
    cards.forEach(c => c.classList.remove("active"));
    btn.classList.add("active");
  };

  // 10. Card Format & Detection
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

    generatedOtp = Math.floor(100000 + Math.random() * 900000).toString();

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

    const paymentLabel = activePaymentMethod === "card" 
      ? "Credit/Debit Card (3D Secure)" 
      : `Instant EFT (${selectedBankName})`;

    // 1. Post order to ASP.NET Core API to validate stock and record into MySQL
    try {
      const orderPayload = {
        orderRef: orderRef,
        customerName: customerName,
        customerEmail: customerEmail,
        deliveryAddress: customerAddress,
        paymentMethod: paymentLabel,
        totalAmount: parseFloat(totalAmount),
        items: cartSnapshot.map(i => ({
          productId: i.id, // Primary key intact
          quantity: i.quantity,
          unitPrice: i.price
        }))
      };

      const apiResponse = await fetch(`${API_BASE_URL}/orders`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(orderPayload)
      });

      const result = await apiResponse.json();

      if (!apiResponse.ok) {
        if (processingView) processingView.style.display = "none";
        closeCheckoutModal();
        alert(`Order Rejected: ${result.message || "Insufficient inventory or error processing order."}`);
        return;
      }

      console.log("Order stored successfully in MySQL database!");
    } catch (apiErr) {
      console.error("Could not post order to backend API:", apiErr);
      if (processingView) processingView.style.display = "none";
      closeCheckoutModal();
      alert("Checkout failed: Could not connect to API server.");
      return;
    }

    // 2. Dispatch final EmailJS receipt ONLY IF backend approved the order
    const emailParams = {
      order_ref: orderRef,
      customer_name: customerName,
      customer_email: customerEmail,
      delivery_address: customerAddress,
      payment_method: paymentLabel,
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