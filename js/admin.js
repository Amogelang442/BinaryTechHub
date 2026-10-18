const API_URL = 'https://binarytechhub-api.onrender.com/api/products';
const BACKEND_ADMIN_KEY = 'BTH-Secret-Admin-2026'; // Passed in X-Admin-Key header

// 5 FIXED GROUP MEMBER CREDENTIALS
const AUTHORIZED_ADMINS = [
    { username: '56087004', password: 'GeraldM@55', displayName: 'Gerald M' },
    { username: '51690489', password: 'TshiamoM@79', displayName: 'Tshiamo M' },
    { username: '53536495', password: 'OratileM@67', displayName: 'Oratile M' },
    { username: '50320777', password: 'MosaC@69', displayName: 'Mosa C' },
    { username: '52711943', password: 'AmogelangM@08', displayName: 'Amogelang M' }
];

document.addEventListener('DOMContentLoaded', checkAuth);

/**
 * Check whether a valid team member is logged in for this browser session
 */
function checkAuth() {
    const activeUser = sessionStorage.getItem('bth_admin_user');
    const loginView = document.getElementById('login-container');
    const dashboardView = document.getElementById('admin-dashboard');

    if (activeUser) {
        if (loginView) loginView.style.display = 'none';
        if (dashboardView) dashboardView.style.display = 'block';

        // Update header badge with current member name
        const userBadge = document.getElementById('admin-current-user');
        if (userBadge) {
            userBadge.textContent = `Logged in as: ${activeUser}`;
        }

        loadInventory();
    } else {
        if (loginView) loginView.style.display = 'block';
        if (dashboardView) dashboardView.style.display = 'none';
    }
}

/**
 * Handle team member login
 */
function handleLogin(event) {
    event.preventDefault();

    const usernameInput = document.getElementById('admin-user').value.trim();
    const passwordInput = document.getElementById('admin-key').value.trim();
    const errorBox = document.getElementById('login-error-msg');

    // Authenticate against the 5 authorized group credentials
    const matchedAdmin = AUTHORIZED_ADMINS.find(admin => 
        admin.username === usernameInput && admin.password === passwordInput
    );

    if (matchedAdmin) {
        sessionStorage.setItem('bth_admin_user', matchedAdmin.displayName);
        sessionStorage.setItem('bth_admin_token', BACKEND_ADMIN_KEY);

        errorBox.style.display = 'none';
        checkAuth();
    } else {
        errorBox.textContent = 'Access Denied: Invalid username or password.';
        errorBox.style.display = 'block';
    }
}

/**
 * Sign out and clear active session
 */
/**
 * Sign out, wipe form inputs, and clear active session
 */
function logoutAdmin() {
    // 1. Remove tokens from session storage
    sessionStorage.removeItem('bth_admin_user');
    sessionStorage.removeItem('bth_admin_token');

    // 2. Clear input fields immediately
    const loginForm = document.getElementById('admin-login-form');
    if (loginForm) {
        loginForm.reset(); // Resets all fields in the form
    }

    const userInput = document.getElementById('admin-user');
    const passInput = document.getElementById('admin-key');
    if (userInput) userInput.value = '';
    if (passInput) passInput.value = '';

    // 3. Clear any leftover error messages
    const errorBox = document.getElementById('login-error-msg');
    if (errorBox) {
        errorBox.textContent = '';
        errorBox.style.display = 'none';
    }

    // 4. Return to the login view
    checkAuth();
}

/**
 * Fetch catalog products from ASP.NET Core API
 */
async function loadInventory() {
    const tbody = document.getElementById('admin-product-rows');

    try {
        const response = await fetch(API_URL);

        if (!response.ok) {
            throw new Error(`API responded with status: ${response.status}`);
        }

        const products = await response.json();
        renderTable(products);
    } catch (error) {
        console.error('Failed to retrieve inventory:', error);
        tbody.innerHTML = `
            <tr>
                <td colspan="6" style="color: #e53e3e; text-align: center; padding: 24px;">
                    <strong>Failed to connect to backend API.</strong><br>
                    Make sure <code>dotnet run</code> is active on <code>http://localhost:5185</code>.
                </td>
            </tr>
        `;
    }
}

/**
 * Render items into the inventory table
 */
function renderTable(products) {
    const tbody = document.getElementById('admin-product-rows');

    if (!products || products.length === 0) {
        tbody.innerHTML = `<tr><td colspan="6" style="text-align: center; padding: 20px;">No products found in database.</td></tr>`;
        return;
    }

    tbody.innerHTML = products.map(product => {
        const id = product.productId || product.product_id;
        const isLowStock = product.stock <= 5;

        return `
            <tr id="row-${id}">
                <td><span class="product-code">${id}</span></td>
                <td><strong>${product.name}</strong></td>
                <td>R ${Number(product.price).toFixed(2)}</td>
                <td id="stock-cell-${id}">
                    <span class="stock-badge ${isLowStock ? 'low' : ''}">
                        ${product.stock} units
                    </span>
                </td>
                <td>
                    <input 
                        type="number" 
                        id="input-${id}" 
                        class="stock-input" 
                        min="0" 
                        value="${product.stock}" 
                    />
                </td>
                <td>
                    <button class="btn-update" id="btn-${id}" onclick="updateStock('${id}')">
                        Save
                    </button>
                </td>
            </tr>
        `;
    }).join('');
}

/**
 * Send PATCH request to API using backend key
 */
async function updateStock(productId) {
    const inputField = document.getElementById(`input-${productId}`);
    const actionButton = document.getElementById(`btn-${productId}`);
    const newStockValue = parseInt(inputField.value, 10);
    const adminKey = sessionStorage.getItem('bth_admin_token');

    if (isNaN(newStockValue) || newStockValue < 0) {
        alert('Please enter a valid stock quantity of 0 or greater.');
        return;
    }

    if (!adminKey) {
        alert('Session expired. Please sign in again.');
        logoutAdmin();
        return;
    }

    actionButton.disabled = true;
    actionButton.textContent = 'Updating...';

    try {
        const response = await fetch(`${API_URL}/${productId}/stock`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'X-Admin-Key': adminKey
            },
            body: JSON.stringify({
                newStock: newStockValue
            })
        });

        const result = await response.json();

        if (response.ok) {
            const stockCell = document.getElementById(`stock-cell-${productId}`);
            const isLowStock = newStockValue <= 5;
            stockCell.innerHTML = `
                <span class="stock-badge ${isLowStock ? 'low' : ''}">
                    ${newStockValue} units
                </span>
            `;

            alert(`Success: ${productId} stock updated to ${newStockValue} in MySQL.`);
        } else {
            alert(`Update rejected: ${result.message || 'Unauthorized or invalid request.'}`);
        }
    } catch (error) {
        console.error('Error dispatching PATCH request:', error);
        alert('Server connection error. Check if the ASP.NET Core API is running.');
    } finally {
        actionButton.disabled = false;
        actionButton.textContent = 'Save';
    }
}