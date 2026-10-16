const API_URL = 'http://localhost:5185/api/products';
const ADMIN_KEY = 'BTH-Secret-Admin-2026';

document.addEventListener('DOMContentLoaded', loadInventory);

/**
 * Fetch all products from the ASP.NET Core API
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
                    <strong>Failed to connect to the backend API.</strong><br>
                    Make sure <code>dotnet run</code> is active in your terminal on <code>http://localhost:5185</code>.
                </td>
            </tr>
        `;
    }
}

/**
 * Render catalog items into the management table
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
 * Send PATCH request to API to update inventory in MySQL
 */
async function updateStock(productId) {
    const inputField = document.getElementById(`input-${productId}`);
    const actionButton = document.getElementById(`btn-${productId}`);
    const newStockValue = parseInt(inputField.value, 10);

    if (isNaN(newStockValue) || newStockValue < 0) {
        alert('Please enter a valid stock quantity of 0 or greater.');
        return;
    }

    actionButton.disabled = true;
    actionButton.textContent = 'Updating...';

    try {
        const response = await fetch(`${API_URL}/${productId}/stock`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'X-Admin-Key': ADMIN_KEY
            },
            body: JSON.stringify({
                newStock: newStockValue
            })
        });

        const result = await response.json();

        if (response.ok) {
            // Update the live stock badge display
            const stockCell = document.getElementById(`stock-cell-${productId}`);
            const isLowStock = newStockValue <= 5;
            stockCell.innerHTML = `
                <span class="stock-badge ${isLowStock ? 'low' : ''}">
                    ${newStockValue} units
                </span>
            `;

            alert(`Success: ${productId} stock updated to ${newStockValue} in MySQL.`);
        } else {
            alert(`Update rejected: ${result.message || 'Unknown error occurred.'}`);
        }
    } catch (error) {
        console.error('Error dispatching PATCH request:', error);
        alert('Server connection error. Check if the ASP.NET Core API is running.');
    } finally {
        actionButton.disabled = false;
        actionButton.textContent = 'Save';
    }
}