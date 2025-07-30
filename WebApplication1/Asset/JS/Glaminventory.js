
// === Glaminventory.js Final Version ===
let products = [];
let stockHistory = [];

let currentPage = 1;
let pageSize = 10;

document.addEventListener('DOMContentLoaded', () => {
    fetchProducts().then(() => {
        updateFilterSuggestions();
        renderProducts();
        renderStock();
        updateStockSummary();
    });
    fetchHistory();
});

async function fetchProducts() {
    try {
        const res = await fetch('/api/admin/products');
        const data = await res.json();
        products = data.map((p, i) => ({
            id: i + 1,
            productId: p.ProductId,
            name: p.Name,
            category: p.Category || 'General',
            brand: p.Type || 'N/A',
            price: p.Price || 0,
            stock: p.Stock || 100,
            minStock: p.MinStock || 5,
            usagePerService: 5,
            size: "100ml",
            lastRestocked: new Date().toISOString().split('T')[0]
        }));
    } catch (err) {
        console.error('Error loading products:', err);
    }
}

async function fetchHistory() {
    try {
        const res = await fetch('/api/admin/products/stock-history');
        const data = await res.json();
        stockHistory = Array.isArray(data) ? data : (data.items || []);
        renderHistory();
    } catch (err) {
        console.error('Error loading history:', err);
        stockHistory = [];
    }
}

function updateFilterSuggestions() {
    const categories = [...new Set(products.map(p => p.category))];
    const brands = [...new Set(products.map(p => p.brand))];

    const catFilter = document.getElementById('categoryFilter');
    const brandFilter = document.getElementById('brandFilter');

    catFilter.innerHTML = `<option value="">All Categories</option>` + categories.map(c => `<option value="${c}">${c}</option>`).join('');
    brandFilter.innerHTML = `<option value="">All Brands</option>` + brands.map(b => `<option value="${b}">${b}</option>`).join('');
}

function renderProducts() {
    const tbody = document.getElementById('productsTable');
    const searchTerm = document.getElementById('productSearch').value.toLowerCase();
    const categoryFilter = document.getElementById('categoryFilter').value;
    const brandFilter = document.getElementById('brandFilter').value;

    let filteredProducts = products.filter(product => {
        const matchesSearch = product.name.toLowerCase().includes(searchTerm);
        const matchesCategory = !categoryFilter || product.category === categoryFilter;
        const matchesBrand = !brandFilter || product.brand === brandFilter;
        return matchesSearch && matchesCategory && matchesBrand;
    });

    tbody.innerHTML = filteredProducts.map(product => `
        <tr>
            <td>${product.name}</td>
            <td>${product.productId}</td>
            <td>${product.category}</td>
            <td>${product.brand}</td>
            <td>₹${product.price}</td>
            <td>${product.stock}</td>
            <td>${getStockStatusBadge(product.stock, product.minStock)}</td>
            <td>
                <div class="action-buttons">
                    <button class="btn btn-edit" onclick="editProduct(${product.id})" title="Edit">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn btn-adjust" onclick="adjustStock(${product.id})" title="Adjust Stock">
                        <i class="fas fa-balance-scale"></i>
                    </button>
                    <button class="btn btn-delete" onclick="deleteProduct(${product.id})" title="Delete">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `).join('');
}

function renderStock() {
    const tbody = document.getElementById('stockTable');
    const search = document.getElementById('stockSearch').value.toLowerCase();
    const statusFilter = document.getElementById('stockStatusFilter').value;

    let filtered = products.filter(product => {
        const matchesSearch = product.name.toLowerCase().includes(search);
        const matchesStatus = !statusFilter || getStockStatusType(product.stock, product.minStock) === statusFilter;
        return matchesSearch && matchesStatus;
    });

    tbody.innerHTML = filtered.map(product => {
        const servicesPossible = product.usagePerService > 0
            ? Math.floor(product.stock * parseInt(product.size || "100") / product.usagePerService)
            : 'N/A';

        return `
            <tr>
                <td>${product.name}</td>
                <td>${product.stock}</td>
                <td>${product.minStock}</td>
                <td>${product.usagePerService}</td>
                <td>${servicesPossible}</td>
                <td>${product.lastRestocked}</td>
                <td>${getStockStatusBadge(product.stock, product.minStock)}</td>
                <td>
                    <button class="btn btn-adjust" onclick="adjustStock(${product.id})">Adjust</button>
                </td>
            </tr>
        `;
    }).join('');
}

function renderHistory() {
    const tbody = document.getElementById('historyTable');
    tbody.innerHTML = stockHistory.map(record => `
        <tr>
            <td>${record.Type}</td>
            <td>${record.Date}</td>
            <td>${record.ProductName}</td>
            <td>${record.Quantity}</td>
            <td>${record.Price}</td>
            <td>${record.CreatedBy}</td>
            <td>${record.StockType}</td>
            <td>${record.LotNo}</td>
            <td>${record.Notes}</td>
        </tr>
    `).join('');
}

function updateStockSummary() {
    const total = products.length;
    const inStock = products.filter(p => p.stock > p.minStock).length;
    const lowStock = products.filter(p => p.stock > 0 && p.stock <= p.minStock).length;
    const outStock = products.filter(p => p.stock === 0).length;

    document.getElementById('totalProducts').textContent = total;
    document.getElementById('inStockCount').textContent = inStock;
    document.getElementById('lowStockCount').textContent = lowStock;
    document.getElementById('outStockCount').textContent = outStock;

    if (lowStock > 0 || outStock > 0) {
        const msg = `${lowStock} products low on stock, ${outStock} out of stock!`;
        alert(msg);
    }
}

function getStockStatusType(stock, minStock) {
    if (stock === 0) return 'out-stock';
    if (stock <= minStock) return 'low-stock';
    return 'in-stock';
}

function getStockStatusBadge(stock, minStock) {
    if (stock === 0) return '<span class="status-badge status-out-stock">Out of Stock</span>';
    if (stock <= minStock) return '<span class="status-badge status-low-stock">Low Stock</span>';
    return '<span class="status-badge status-in-stock">In Stock</span>';
}

function editProduct(id) {
    const product = products.find(p => p.id === id);
    console.log('Edit:', product);
}

function deleteProduct(id) {
    if (confirm('Delete this product?')) {
        products = products.filter(p => p.id !== id);
        renderProducts();
        renderStock();
        updateStockSummary();
    }
}

function adjustStock(id) {
    const product = products.find(p => p.id === id);
    console.log('Adjust Stock:', product);
}
