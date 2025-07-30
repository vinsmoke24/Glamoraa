// Global variables
let selectedServices = [];
let selectedProducts = [];
let currentCustomer = null;
let staffList = [];
let servicesList = [];
let productsList = [];

// API Base URL
const API_BASE = '/api/invoices';

// Initialize the application
document.addEventListener('DOMContentLoaded', function () {
    initializeApp();
});

async function initializeApp() {
    try {
        // Set today's date
        const today = new Date().toISOString().split('T')[0];
        document.getElementById('invoiceDate').value = today;

        // Load initial data
        await Promise.all([
            loadServices(),
            loadProducts(),
            loadStaff(),
            loadNextInvoiceNumber(),
            loadPreviousInvoices()
        ]);

        console.log('Application initialized successfully');
    } catch (error) {
        console.error('Error initializing application:', error);
        showError('Failed to initialize application');
    }
}

// Load services from API
async function loadServices() {
    try {
        const response = await fetch(`${API_BASE}/services`);
        if (!response.ok) throw new Error('Failed to load services');

        servicesList = await response.json();
        populateServicesDropdown();
        populateServicesGrid();
    } catch (error) {
        console.error('Error loading services:', error);
        showError('Failed to load services');
    }
}

// Load products from API
async function loadProducts() {
    try {
        const response = await fetch(`${API_BASE}/products`);
        if (!response.ok) throw new Error('Failed to load products');

        productsList = await response.json();
        populateProductsDropdown();
    } catch (error) {
        console.error('Error loading products:', error);
        showError('Failed to load products');
    }
}

// Load staff from API
async function loadStaff() {
    try {
        const response = await fetch(`${API_BASE}/staff`);
        if (!response.ok) throw new Error('Failed to load staff');

        staffList = await response.json();
        populateStaffDropdown();
    } catch (error) {
        console.error('Error loading staff:', error);
        showError('Failed to load staff');
    }
}

// Load next invoice number
async function loadNextInvoiceNumber() {
    try {
        const response = await fetch(`${API_BASE}/next-number`);
        if (!response.ok) throw new Error('Failed to load invoice number');

        const data = await response.json();
        document.getElementById('invoiceNumber').value = data.InvoiceNumber;
    } catch (error) {
        console.error('Error loading invoice number:', error);
        showError('Failed to load invoice number');
    }
}

// Search customer by phone
async function searchCustomerByPhone() {
    const phone = document.getElementById('clientPhone').value.trim();
    if (!phone) return;

    try {
        const response = await fetch(`${API_BASE}/customer/${encodeURIComponent(phone)}`);

        if (response.ok) {
            currentCustomer = await response.json();
            populateCustomerData();
        } else if (response.status === 404) {
            // Customer not found - clear fields for new customer
            currentCustomer = null;
            document.getElementById('clientName').value = '';
            document.getElementById('clientEmail').value = '';
            showInfo('Customer not found. Please enter new customer details.');
        } else {
            throw new Error('Failed to search customer');
        }
    } catch (error) {
        console.error('Error searching customer:', error);
        showError('Failed to search customer');
    }
}

// Populate customer data
function populateCustomerData() {
    if (!currentCustomer) return;

    document.getElementById('clientName').value = currentCustomer.FullName || '';
    document.getElementById('clientEmail').value = currentCustomer.Email || '';

    updateClientDisplay();
}

// Populate services dropdown
function populateServicesDropdown() {
    const dropdown = document.getElementById('servicesDropdown');
    dropdown.innerHTML = '<option value="">Select a service to add...</option>';

    servicesList.forEach(service => {
        service.SubServices.forEach(subService => {
            const option = document.createElement('option');
            option.value = JSON.stringify({
                serviceId: service.ServiceId,
                subServiceId: subService.SubServiceId,
                name: `${service.ServiceName} - ${subService.SubServiceName}`,
                price: subService.Price
            });
            option.textContent = `${service.ServiceName} - ${subService.SubServiceName} - ₹${subService.Price}`;
            dropdown.appendChild(option);
        });
    });
}

// Populate products dropdown
function populateProductsDropdown() {
    // Add products section to HTML if not exists
    const productsSection = document.querySelector('.products-section');
    if (!productsSection) {
        const servicesSection = document.querySelector('.services-section');
        const productsHTML = `
            <div class="products-section">
                <h2>Add Products</h2>
                <div class="form-group">
                    <label>Select Product</label>
                    <select id="productsDropdown" class="products-dropdown" onchange="addProductFromDropdown()">
                        <option value="">Select a product to add...</option>
                    </select>
                </div>
                <div id="selectedProducts" class="selected-products"></div>
            </div>
        `;
        servicesSection.insertAdjacentHTML('afterend', productsHTML);
    }

    const dropdown = document.getElementById('productsDropdown');
    if (dropdown) {
        dropdown.innerHTML = '<option value="">Select a product to add...</option>';

        productsList.forEach(product => {
            const option = document.createElement('option');
            option.value = JSON.stringify({
                productId: product.ProductId,
                name: product.Name,
                price: product.Price,
                stockQuantity: product.StockQuantity
            });
            option.textContent = `${product.Name} - ₹${product.Price} (Stock: ${product.StockQuantity})`;
            dropdown.appendChild(option);
        });
    }
}

// Populate staff dropdown
function populateStaffDropdown() {
    const dropdown = document.getElementById('stylist');
    dropdown.innerHTML = '';

    staffList.forEach(staff => {
        const option = document.createElement('option');
        option.value = staff.StaffId;
        option.textContent = staff.FullName;
        dropdown.appendChild(option);
    });
}

// Add service from dropdown
function addServiceFromDropdown() {
    const dropdown = document.getElementById('servicesDropdown');
    const selectedValue = dropdown.value;

    if (selectedValue) {
        const serviceData = JSON.parse(selectedValue);

        // Check if service is already added
        const existingService = selectedServices.find(service =>
            service.serviceId === serviceData.serviceId &&
            service.subServiceId === serviceData.subServiceId
        );

        if (existingService) {
            showError('This service is already added!');
            dropdown.value = '';
            return;
        }

        // Add service to array
        selectedServices.push({
            serviceId: serviceData.serviceId,
            subServiceId: serviceData.subServiceId,
            name: serviceData.name,
            price: serviceData.price,
            gstRate: 18, // Default GST rate
            staffId: staffList.length > 0 ? staffList[0].StaffId : null
        });

        // Reset dropdown
        dropdown.value = '';

        // Update display
        updateServicesDisplay();
    }
}

// Add product from dropdown
function addProductFromDropdown() {
    const dropdown = document.getElementById('productsDropdown');
    const selectedValue = dropdown.value;

    if (selectedValue) {
        const productData = JSON.parse(selectedValue);

        // Check if product is already added
        const existingProduct = selectedProducts.find(product =>
            product.productId === productData.productId
        );

        if (existingProduct) {
            showError('This product is already added!');
            dropdown.value = '';
            return;
        }

        // Add product to array
        selectedProducts.push({
            productId: productData.productId,
            name: productData.name,
            price: productData.price,
            quantity: 1,
            gstRate: 18, // Default GST rate
            staffId: staffList.length > 0 ? staffList[0].StaffId : null
        });

        // Reset dropdown
        dropdown.value = '';

        // Update display
        updateProductsDisplay();
    }
}

// Update services display
function updateServicesDisplay() {
    const servicesDisplay = document.getElementById('servicesDisplay');
    const servicesListDisplay = document.getElementById('servicesListDisplay');
    const servicesTotalDisplay = document.getElementById('servicesTotalDisplay');

    if (selectedServices.length > 0) {
        servicesDisplay.style.display = 'block';

        let servicesHTML = '';
        let total = 0;

        selectedServices.forEach((service, index) => {
            total += service.price;
            servicesHTML += `
                <div class="service-item">
                    <span class="service-name">${service.name}</span>
                    <div>
                        <span class="service-price">₹${service.price}</span>
                        <button class="remove-service" onclick="removeService(${index})">×</button>
                    </div>
                </div>
            `;
        });

        servicesListDisplay.innerHTML = servicesHTML;
        servicesTotalDisplay.innerHTML = `Total: ₹${total}`;
    } else {
        servicesDisplay.style.display = 'none';
    }
}

// Update products display
function updateProductsDisplay() {
    const productsContainer = document.getElementById('selectedProducts');
    if (!productsContainer) return;

    if (selectedProducts.length > 0) {
        let productsHTML = '<h3>Selected Products</h3>';
        let total = 0;

        selectedProducts.forEach((product, index) => {
            const lineTotal = product.price * product.quantity;
            total += lineTotal;
            productsHTML += `
                <div class="product-item">
                    <span class="product-name">${product.name}</span>
                    <input type="number" min="1" value="${product.quantity}" 
                           onchange="updateProductQuantity(${index}, this.value)">
                    <span class="product-price">₹${product.price} × ${product.quantity} = ₹${lineTotal}</span>
                    <button class="remove-product" onclick="removeProduct(${index})">×</button>
                </div>
            `;
        });

        productsHTML += `<div class="products-total">Total: ₹${total}</div>`;
        productsContainer.innerHTML = productsHTML;
    } else {
        productsContainer.innerHTML = '';
    }
}

// Remove service
function removeService(index) {
    selectedServices.splice(index, 1);
    updateServicesDisplay();
}

// Remove product
function removeProduct(index) {
    selectedProducts.splice(index, 1);
    updateProductsDisplay();
}

// Update product quantity
function updateProductQuantity(index, quantity) {
    selectedProducts[index].quantity = parseInt(quantity) || 1;
    updateProductsDisplay();
}

// Create new invoice
async function createNewInvoice() {
    // Clear all fields
    document.getElementById('clientPhone').value = '';
    document.getElementById('clientName').value = '';
    document.getElementById('clientEmail').value = '';
    selectedServices = [];
    selectedProducts = [];
    currentCustomer = null;

    // Reset displays
    updateClientDisplay();
    updateServicesDisplay();
    updateProductsDisplay();

    // Load new invoice number
    await loadNextInvoiceNumber();
}

// Save invoice
async function saveInvoice() {
    const clientName = document.getElementById('clientName').value.trim();
    const clientPhone = document.getElementById('clientPhone').value.trim();
    const clientEmail = document.getElementById('clientEmail').value.trim();
    const stylistId = document.getElementById('stylist').value;

    // Validation
    if (!clientName || !clientPhone) {
        showError('Please fill in client name and phone number.');
        return;
    }

    if (selectedServices.length === 0 && selectedProducts.length === 0) {
        showError('Please add at least one service or product.');
        return;
    }

    try {
        // Prepare invoice data
        const invoiceData = {
            CustomerId: currentCustomer ? currentCustomer.CustomerId : null,
            AppointmentId: null, // Set if linking to an appointment
            Services: selectedServices.map(service => ({
                AppointmentServiceId: service.subServiceId,
                StaffId: service.staffId || stylistId,
                PriceAtBooking: service.price,
                GstRatePercent: service.gstRate
            })),
            Products: selectedProducts.map(product => ({
                ProductId: product.productId,
                StaffId: product.staffId || stylistId,
                Quantity: product.quantity,
                UnitPrice: product.price,
                GstRatePercent: product.gstRate
            })),
            CouponCode: '', // Add coupon functionality if needed
            TokenPaid: 0, // Add token payment functionality if needed
            CreatedBy: 'System' // Replace with actual user
        };

        // Create invoice
        const response = await fetch(API_BASE, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(invoiceData)
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Failed to create invoice');
        }

        const createdInvoice = await response.json();

        showSuccess(`Invoice created successfully! Invoice ID: ${createdInvoice.InvoiceId}`);

        // Refresh invoice list
        await loadPreviousInvoices();

        // Reset form
        await createNewInvoice();

    } catch (error) {
        console.error('Error creating invoice:', error);
        showError('Failed to create invoice: ' + error.message);
    }
}

// Load previous invoices
async function loadPreviousInvoices() {
    try {
        const response = await fetch(API_BASE);
        if (!response.ok) throw new Error('Failed to load invoices');

        const invoices = await response.json();
        updateInvoiceTable(invoices);
    } catch (error) {
        console.error('Error loading invoices:', error);
        showError('Failed to load invoices');
    }
}

// Update invoice table
function updateInvoiceTable(invoices) {
    const tableBody = document.getElementById('invoiceTableBody');
    if (!tableBody) return;

    let tableHTML = '';

    invoices.forEach(invoice => {
        const date = new Date(invoice.InvoiceDate).toLocaleDateString();
        const statusClass = invoice.PaymentStatus.toLowerCase() === 'paid' ? 'status-paid' : 'status-pending';

        tableHTML += `
            <tr>
                <td>${invoice.InvoiceId}</td>
                <td>${date}</td>
                <td>${invoice.CustomerName}</td>
                <td>${invoice.Services.join(', ')}</td>
                <td>₹${invoice.TotalAmount}</td>
                <td>-</td>
                <td><span class="status-badge ${statusClass}">${invoice.PaymentStatus}</span></td>
                <td>
                    <button class="btn btn-secondary" style="padding: 5px 10px; font-size: 12px;" 
                            onclick="viewInvoice('${invoice.InvoiceId}')">View</button>
                </td>
            </tr>
        `;
    });

    tableBody.innerHTML = tableHTML;
}

// View invoice details
async function viewInvoice(invoiceId) {
    try {
        const response = await fetch(`${API_BASE}/${invoiceId}`);
        if (!response.ok) throw new Error('Failed to load invoice details');

        const invoice = await response.json();

        // Create a simple modal or alert with invoice details
        const services = invoice.ServiceLines.map(s => s.ServiceName).join(', ');
        const products = invoice.ProductLines.map(p => `${p.ProductName} (${p.Quantity})`).join(', ');

        alert(`Invoice Details:\n\nInvoice ID: ${invoice.InvoiceId}\nDate: ${new Date(invoice.InvoiceDate).toLocaleDateString()}\nServices: ${services}\nProducts: ${products}\nSubtotal: ₹${invoice.SubTotal}\nDiscount: ₹${invoice.Discount}\nTax: ₹${invoice.TaxAmount}\nTotal: ₹${invoice.TotalAmount}`);

    } catch (error) {
        console.error('Error viewing invoice:', error);
        showError('Failed to load invoice details');
    }
}

// Page navigation functions
function showPreviousInvoices() {
    document.getElementById('mainPage').classList.remove('active');
    document.getElementById('previousInvoicesPage').classList.add('active');
}

function showMainPage() {
    document.getElementById('previousInvoicesPage').classList.remove('active');
    document.getElementById('mainPage').classList.add('active');
}

// Client display update
function updateClientDisplay() {
    const phone = document.getElementById('clientPhone').value || 'Not provided';
    const name = document.getElementById('clientName').value || 'Not provided';
    const email = document.getElementById('clientEmail').value || 'Not provided';

    document.getElementById('displayPhone').textContent = phone;
    document.getElementById('displayName').textContent = name;
    document.getElementById('displayEmail').textContent = email;
}

// Utility functions
function showError(message) {
    alert('Error: ' + message);
}

function showSuccess(message) {
    alert('Success: ' + message);
}

function showInfo(message) {
    alert('Info: ' + message);
}

// Export invoices
function exportInvoices() {
    showInfo('Export functionality coming soon!');
}

// Event listeners
document.getElementById('clientPhone').addEventListener('blur', searchCustomerByPhone);
document.getElementById('clientPhone').addEventListener('input', updateClientDisplay);
document.getElementById('clientName').addEventListener('input', updateClientDisplay);
document.getElementById('clientEmail').addEventListener('input', updateClientDisplay);