const tableBody = document.getElementById('serviceTableBody');
const pageInfo = document.getElementById('pageInfo');
const prevBtn = document.getElementById('prevBtn');
const nextBtn = document.getElementById('nextBtn');

// Edit modal references
const editModal = document.getElementById('editServiceModal');
const editForm = document.getElementById('editServiceForm');
const editServiceId = document.getElementById('editServiceId');
const editSubServiceId = document.getElementById('editSubServiceId');
const editSubName = document.getElementById('editSubserviceName');
const editSubPrice = document.getElementById('editServicePrice');
const editSubDuration = document.getElementById('editServiceDuration');
const editSubDesc = document.getElementById('editServiceDescription');
const editSubStatus = document.getElementById('editServiceStatus');

// Add modal references
const addServiceModal = document.getElementById('addServiceModal');
const addSubServiceModal = document.getElementById('addSubserviceModal');
const addServiceForm = document.getElementById('addServiceForm');
const serviceNameEl = document.getElementById('serviceName');

let currentPage = 1;
const itemsPerPage = 5;
let allServices = [];

document.addEventListener('DOMContentLoaded', () => {
    loadServiceData();
});

function loadServiceData() {
    fetch('https://localhost:44353/api/admin/services')
        .then(res => res.json())
        .then(data => {
            allServices = data;
            renderTablePage(currentPage);
        })
        .catch(error => {
            console.error("Error fetching services:", error);
            alert("Error loading services.");
        });
}

function renderTablePage(page) {
    const start = (page - 1) * itemsPerPage;
    const end = start + itemsPerPage;
    const paginatedServices = allServices.slice(start, end);

    tableBody.innerHTML = ''; // Clear existing table data
    paginatedServices.forEach(service => {
        const subCount = service.SubServices.length;

        service.SubServices.forEach((sub, index) => {
            const row = document.createElement('tr');

            if (index === 0) {
                row.innerHTML += `
                    <td rowspan="${subCount}">
                        ${service.ServiceName}<br>
                        <button class="btn btn-add btn-sm" onclick="openAddSubserviceModal()">+ Add Subservice</button>
                    </td>`;
            }

            row.innerHTML += `
                <td>${sub.SubServiceName}</td>
                <td>₹${sub.Price}</td>
                <td>${sub.DurationMinutes} min</td>
                <td>${sub.Description || ''}</td>
                <td><span class="status ${sub.Status ? 'status-available' : 'status-unavailable'}">${sub.Status ? 'Active' : 'Inactive'}</span></td>
                <td>
                    <button class="action-btn btn-edit" onclick="editRow('${service.ServiceId}', ${sub.SubServiceId})" title="Edit Service">✏️</button>
                    <button class="action-btn btn-delete" onclick="deleteSubservice('${service.ServiceId}', ${sub.SubServiceId}, this)" title="Delete Service" ${!sub.Status ? 'disabled' : ''}>🗑️</button>
                </td>`;

            tableBody.appendChild(row);
        });
    });

    const totalPages = Math.ceil(allServices.length / itemsPerPage);
    if (pageInfo) pageInfo.textContent = `Page ${currentPage} of ${totalPages}`;
    if (prevBtn) prevBtn.disabled = currentPage === 1;
    if (nextBtn) nextBtn.disabled = currentPage === totalPages;
}

function changePage(direction) {
    currentPage += direction;
    renderTablePage(currentPage);
}

function deleteSubservice(serviceId, subServiceId, btnRef) {
    if (!confirm('Are you sure you want to delete this subservice?')) return;

    fetch(`https://localhost:44353/api/admin/services/${serviceId}/subservices/${subServiceId}`, {
        method: 'DELETE'
    })
        .then(response => response.json())
        .then(data => {
            alert(data.Message || 'Subservice deleted successfully.');
            loadServiceData();
        })
        .catch(error => {
            console.error("Error deleting subservice:", error);
            alert("Error deleting subservice: " + error.message);
        });
}

function editRow(serviceId, subServiceId) {
    const service = allServices.find(s => s.ServiceId === serviceId);
    if (!service) {
        console.log("Service not found for id:", serviceId);
        return;
    }

    const sub = service.SubServices.find(x => x.SubServiceId === subServiceId);
    if (!sub) {
        console.log("Subservice not found for id:", subServiceId);
        return;
    }

    // Populate the form fields with the existing data
    editServiceId.value = serviceId;
    editSubServiceId.value = subServiceId;
    editSubName.value = sub.SubServiceName;
    editSubPrice.value = sub.Price;
    editSubDuration.value = sub.DurationMinutes;
    editSubDesc.value = sub.Description;
    editSubStatus.value = sub.Status ? 'Active' : 'Inactive';  // Fix the status value

    // Show the edit modal
    editModal.style.display = 'flex';
}

if (editForm) {
    editForm.addEventListener('submit', function (e) {
        e.preventDefault();
        const payload = {
            SubServiceName: editSubName.value,
            Price: parseFloat(editSubPrice.value),
            DurationMinutes: parseInt(editSubDuration.value),
            Description: editSubDesc.value,
            Status: editSubStatus.value === 'Active'  // Correctly handle Active/Inactive
        };

        fetch(`https://localhost:44353/api/admin/services/${editServiceId.value}/subservices/${editSubServiceId.value}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        }).then(res => res.json()).then(data => {
            alert(data.Message);
            closeEditServiceModal();
            loadServiceData();
        }).catch(error => {
            console.error("Error updating subservice:", error);
            alert("Error updating subservice.");
        });
    });
}

function closeEditServiceModal() {
    editModal.style.display = 'none';
}

function openAddServiceModal() {
    addServiceModal.style.display = 'flex';
}

function closeAddServiceModal() {
    addServiceModal.style.display = 'none';
    addServiceForm.reset();  // Reset the form on modal close
}

if (addServiceForm) {
    addServiceForm.addEventListener('submit', async function (e) {
        e.preventDefault();

        const statusValue = document.getElementById('serviceStatus').value;

        // Create the service payload
        const servicePayload = {
            ServiceName: serviceNameEl.value,
            Description: document.getElementById('serviceDescription').value || '',
            Status: statusValue === 'Active'  // ✅ For Service only: bit true/false
        };

        // Call the API to add the service
        const res = await fetch('/api/admin/services', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(servicePayload)
        });

        if (!res.ok) {
            const errorText = await res.text();
            console.error("Add Service failed:", errorText);
            alert("Failed to add Service. Check server logs.");
            return;
        }

        const serviceData = await res.json();
        if (!serviceData.ServiceId) {
            alert("Server did not return ServiceId.");
            return;
        }

        // Now that the service is added, add the corresponding subservice
        const subPayload = {
            SubServiceName: document.getElementById('subserviceName').value,
            Price: parseFloat(document.getElementById('servicePrice').value),
            DurationMinutes: parseInt(document.getElementById('serviceDuration').value),
            Description: document.getElementById('serviceDescription').value || '',
            Status: statusValue   // ✅ Send "Active"/"Inactive" exactly
        };

        const subRes = await fetch(`/api/admin/services/${serviceData.ServiceId}/subservices`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(subPayload)
        });

        if (!subRes.ok) {
            const subError = await subRes.text();
            console.error("Add SubService failed:", subError);
            alert("SubService creation failed. Service is created, but SubService needs retry.");
            return;
        }

        // Success message
        alert("Service and SubService added successfully!");

        // Close the modal after success
        closeAddServiceModal();

        // Immediately reload the service data to reflect the new service
        loadServiceData();
    });
}

// Open Add Subservice Modal and set the ServiceId
function openAddSubserviceModal(serviceId, serviceName) {
    const serviceIdInput = document.getElementById('serviceIdForSubservice');
    serviceIdInput.value = serviceId;  // Set the selected ServiceId

    // Optionally, you can display the service name in the modal header or somewhere else
    const modalHeader = document.querySelector('#addSubserviceModal .modal-header h2');
    modalHeader.innerHTML = `Add New Subservice to: ${serviceName}`;

    addSubserviceModal.style.display = 'flex';  // Show the modal
}

// Close Add Subservice Modal
function closeAddSubserviceModal() {
    addSubserviceModal.style.display = 'none';
    document.getElementById('addSubserviceForm').reset();  // Reset form inputs
}

// Handle form submission to add subservice
if (addSubserviceForm) {
    addSubserviceForm.addEventListener('submit', async function (e) {
        e.preventDefault();

        const serviceId = document.getElementById('serviceIdForSubservice').value;
        const statusValue = document.getElementById('subserviceStatus').value;

        const subservicePayload = {
            SubServiceName: document.getElementById('subserviceName').value,
            Price: parseFloat(document.getElementById('subservicePrice').value),
            DurationMinutes: parseInt(document.getElementById('subserviceDuration').value),
            Description: document.getElementById('subserviceDescription').value || '',
            Status: statusValue === 'Active'  // Store the boolean value
        };

        // Add subservice to the selected service
        const res = await fetch(`/api/admin/services/${serviceId}/subservices`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(subservicePayload)
        });

        if (!res.ok) {
            const errorText = await res.text();
            console.error("Add SubService failed:", errorText);
            alert("Failed to add Subservice. Check server logs.");
            return;
        }

        const subserviceData = await res.json();
        alert("Subservice added successfully!");

        // Close the modal
        closeAddSubserviceModal();

        // Reload services data to show the new subservice
        loadServiceData();
    });
}
