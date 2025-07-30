let customerData = [], currentPage = 1, pageSize = 10;

// 🔑 Load token & context ONCE
const token = localStorage.getItem('token');
const salonId = localStorage.getItem('salonId');
const branchId = localStorage.getItem('branchId');

document.addEventListener('DOMContentLoaded', () => {
    fetchCustomers();
    setupEventListeners();
});

async function fetchCustomers() {
    try {
        const res = await fetch(`/api/customers?salonId=${salonId}&branchId=${branchId}`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!res.ok) throw new Error('Failed to fetch');

        const data = await res.json();
        customerData = data.map(c => ({
            id: c.CustomerId,
            name: c.FullName,
            phone: c.PhoneNumber,
            email: c.Email,
            gender: c.Gender || '-',
            lastVisit: c.LastVisitDate?.split('T')[0] || '-',
            amount: c.AmountSpent || 0,
            status: c.Status,
            membership: c.IsMember ? 'Premium' : 'Regular'
        }));
        currentPage = 1;
        renderCustomerTable();
        updateStats();
    } catch (err) {
        console.error(err);
        showToast("Error fetching customers", "error");
    }
}

function renderCustomerTable() {
    const tbody = document.getElementById('clientsTableBody');
    tbody.innerHTML = '';

    const search = document.getElementById('clientSearch').value.toLowerCase();
    const filter = document.getElementById('membershipFilter').value;

    const filtered = customerData.filter(c =>
        (c.name.toLowerCase().includes(search) || c.phone.includes(search) || c.email.toLowerCase().includes(search))
        && (filter === 'all' || c.membership === filter)
    );

    const start = (currentPage - 1) * pageSize;
    const pageData = filtered.slice(start, start + pageSize);

    pageData.forEach(c => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${escapeHtml(c.name)}</td>
            <td>${escapeHtml(c.phone)}</td>
            <td>${escapeHtml(c.email)}</td>
            <td>${c.lastVisit}</td>
            <td>${c.amount}</td>
            <td>${c.status}</td>
            <td>
                <button class="btn btn-view" onclick="viewCustomer('${c.id}')"><i class="fas fa-eye"></i></button>
                <button class="btn btn-edit" onclick="editCustomer('${c.id}')"><i class="fas fa-edit"></i></button>
                <button class="btn btn-danger" onclick="deleteCustomer('${c.id}')"><i class="fas fa-trash"></i></button>
            </td>`;
        tbody.appendChild(row);
    });

    document.getElementById('pageIndicator').innerText = `Page ${currentPage} of ${Math.ceil(filtered.length / pageSize)}`;
}

function updateStats() {
    document.getElementById('totalClients').innerText = customerData.length;
    document.getElementById('premiumMembers').innerText = customerData.filter(c => c.membership === 'Premium').length;
    document.getElementById('maleClients').innerText = customerData.filter(c => c.gender === 'Male').length;
    document.getElementById('femaleClients').innerText = customerData.filter(c => c.gender === 'Female').length;
}

function setupEventListeners() {
    document.getElementById('clientSearch').addEventListener('input', () => {
        currentPage = 1;
        renderCustomerTable();
    });
    document.getElementById('membershipFilter').addEventListener('change', () => {
        currentPage = 1;
        renderCustomerTable();
    });
    document.getElementById('prevPage').addEventListener('click', () => {
        if (currentPage > 1) {
            currentPage--;
            renderCustomerTable();
        }
    });
    document.getElementById('nextPage').addEventListener('click', () => {
        currentPage++;
        renderCustomerTable();
    });
    document.getElementById('exportCSV').addEventListener('click', exportToCSV);
    document.getElementById('addButton').addEventListener('click', () => openCustomerForm());
}

function exportToCSV() {
    let csv = 'Name,Phone,Email,Last Visit,Amount,Status,Membership\n';
    customerData.forEach(c => {
        csv += `"${c.name}","${c.phone}","${c.email}","${c.lastVisit}","${c.amount}","${c.status}","${c.membership}"\n`;
    });
    const blob = new Blob([csv], { type: 'text/csv' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'clients.csv';
    a.click();
    URL.revokeObjectURL(url);
}

function viewCustomer(id) {
    const c = customerData.find(c => c.id === id);
    if (!c) return;
    const html = `
        <p><b>Name:</b> ${c.name}</p>
        <p><b>Phone:</b> ${c.phone}</p>
        <p><b>Email:</b> ${c.email}</p>
        <p><b>Gender:</b> ${c.gender}</p>
        <p><b>Membership:</b> ${c.membership}</p>
        <p><b>Amount Spent:</b> ₹${c.amount}</p>
        <p><b>Status:</b> ${c.status}</p>`;
    showModal("Customer Details", html);
}

function editCustomer(id) {
    const c = customerData.find(c => c.id === id);
    if (!c) return;
    openCustomerForm(c);
}

async function deleteCustomer(id) {
    if (confirm('Are you sure you want to delete this customer?')) {
        const res = await fetch(`/api/customers/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` }
        });
        if (res.ok) {
            showToast('Customer deleted!', 'success');
            fetchCustomers();
        } else {
            showToast('Error deleting customer', 'error');
        }
    }
}

function openCustomerForm(c = null) {
    const isEdit = !!c;
    const formHtml = `
        <form id="customerForm">
            <div class="modal-form-group">
                <label>Name *</label>
                <input type="text" name="name" value="${isEdit ? c.name : ''}" required/>
            </div>
            <div class="modal-form-group">
                <label>Phone *</label>
                <input type="text" name="phone" value="${isEdit ? c.phone : ''}" required/>
            </div>
            <div class="modal-form-group">
                <label>Email *</label>
                <input type="email" name="email" value="${isEdit ? c.email : ''}" required/>
            </div>
            <div class="modal-form-group">
                <label>Gender *</label>
                <select name="gender" required>
                    <option value="">-- Select Gender --</option>
                    <option value="Male" ${isEdit && c.gender === 'Male' ? 'selected' : ''}>Male</option>
                    <option value="Female" ${isEdit && c.gender === 'Female' ? 'selected' : ''}>Female</option>
                    <option value="Other" ${isEdit && c.gender === 'Other' ? 'selected' : ''}>Other</option>
                </select>
            </div>
            <div class="modal-form-group">
                <label>Membership *</label>
                <select name="membership" required>
                    <option value="Regular" ${isEdit && c.membership === 'Regular' ? 'selected' : ''}>Regular</option>
                    <option value="Premium" ${isEdit && c.membership === 'Premium' ? 'selected' : ''}>Premium</option>
                </select>
            </div>
            <div class="modal-form-group">
                <label>Status *</label>
                <select name="status" required>
                    <option value="Active" ${isEdit && c.status === 'Active' ? 'selected' : ''}>Active</option>
                    <option value="Inactive" ${isEdit && c.status === 'Inactive' ? 'selected' : ''}>Inactive</option>
                </select>
            </div>
            <div class="modal-form-group">
                <label>Amount Spent</label>
                <input type="number" name="amount" value="${isEdit ? c.amount : 0}" step="0.01"/>
            </div>
            <div class="modal-actions">
                <button type="button" class="btn btn-secondary" onclick="closeModal()">Cancel</button>
                <button type="submit" class="btn btn-primary">Save</button>
            </div>
        </form>`;
    showModal(isEdit ? "Edit Customer" : "Add Customer", formHtml);

    document.getElementById('customerForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        const fd = new FormData(e.target);
        const payload = {
            FullName: fd.get('name'),
            PhoneNumber: fd.get('phone'),
            Email: fd.get('email'),
            Gender: fd.get('gender'),
            IsMember: fd.get('membership') === 'Premium',
            LastVisitDate: new Date().toISOString(),
            AmountSpent: parseFloat(fd.get('amount')) || 0,
            Status: fd.get('status'),
            MembershipId: null,
            // ✅ Pass context if needed
            SalonId: salonId,
            BranchId: branchId
        };

        const url = isEdit ? `/api/customers/${c.id}` : '/api/customers';
        const method = isEdit ? 'PUT' : 'POST';

        const res = await fetch(url, {
            method: method,
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        if (res.ok) {
            showToast('Customer saved!', 'success');
            fetchCustomers();
            closeModal();
        } else {
            showToast('Error saving customer', 'error');
        }
    });
}

function showModal(title, content) {
    const overlay = document.getElementById('modalOverlay');
    overlay.innerHTML = `<div class="modal-box">
        <button class="modal-close" onclick="closeModal()">&times;</button>
        <h3>${title}</h3>
        ${content}
    </div>`;
    overlay.classList.add('active');
    document.body.style.overflow = 'hidden';
}

function closeModal() {
    const overlay = document.getElementById('modalOverlay');
    overlay.classList.remove('active');
    document.body.style.overflow = '';
}

function showToast(msg, type = 'info') {
    const toast = document.getElementById('toast');
    toast.textContent = `${type.toUpperCase()}: ${msg}`;
    toast.className = 'toast show';
    setTimeout(() => { toast.className = 'toast'; }, 3000);
}

function escapeHtml(text) {
    const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
    return text.replace(/[&<>"']/g, m => map[m]);
}
