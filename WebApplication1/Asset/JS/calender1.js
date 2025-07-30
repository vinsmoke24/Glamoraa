// 📅 calendarApp.js – Handles Calendar UI & Appointment API Integration

let currentMonth, currentYear, selectedDate;

const monthNames = [
    "January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
];

// Initialize Current Date
function initCurrentDate() {
    const today = new Date();
    currentYear = today.getFullYear();
    currentMonth = today.getMonth();
    selectedDate = today.getDate();
}

// Update Calendar Title (Month & Year)
function updateCalendarTitle() {
    document.getElementById('calendarTitle').textContent = `${monthNames[currentMonth]} ${currentYear}`;
}

// Format Date to ISO (yyyy-mm-dd)
function formatDateToISO(year, month, day) {
    return `${year}-${String(month + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
}

// Check if Selected Date is in the Past
function isPastDate(year, month, day) {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const selected = new Date(year, month, day);
    return selected < today;
}

// Handle Date Selection on the Calendar
function selectDate(date) {
    document.querySelectorAll('.calendar-day').forEach(day => day.classList.remove('selected'));
    const selectedDay = document.querySelector(`[data-date="${date}"]`);
    if (selectedDay) {
        selectedDay.classList.add('selected');
        selectedDate = date;
        const isoDate = formatDateToISO(currentYear, currentMonth, selectedDate);
        loadAppointments(isoDate);
    }
}

// Move to the Previous Month
function previousMonth() {
    if (currentMonth === 0) {
        currentMonth = 11;
        currentYear--;
    } else {
        currentMonth--;
    }
    updateCalendarTitle();
}

// Move to the Next Month
function nextMonth() {
    if (currentMonth === 11) {
        currentMonth = 0;
        currentYear++;
    } else {
        currentMonth++;
    }
    updateCalendarTitle();
}

// Open Add Appointment Modal and Populate Dropdowns
function openAddAppointmentModal(date = null) {
    if (isPastDate(currentYear, currentMonth, date ?? selectedDate)) {
        alert("You cannot book an appointment in the past.");
        return;
    }

    const modal = document.getElementById('addAppointmentModal');
    if (!modal) return;
    modal.style.display = 'block';

    if (date) {
        document.getElementById('appointmentDate').value = formatDateToISO(currentYear, currentMonth, date);
    }

    populateServiceDropdown();
    populateStylistDropdown();
}

// Close Add Appointment Modal
function closeAddAppointmentModal() {
    const modal = document.getElementById('addAppointmentModal');
    if (modal) {
        modal.style.display = 'none';
        document.getElementById('addAppointmentForm').reset();
    }
}

// Handle Clicks Outside Modal to Close it
function handleOutsideModalClicks(event) {
    const modals = ['searchModal', 'filterModal', 'salesModal', 'addAppointmentModal'];
    modals.forEach(id => {
        const modal = document.getElementById(id);
        if (event.target === modal) {
            if (id === 'searchModal') closeSearchModal();
            else if (id === 'filterModal') closeFilterModal();
            else if (id === 'salesModal') closeSalesModal();
            else if (id === 'addAppointmentModal') closeAddAppointmentModal();
        }
    });
}

// Bind Calendar Events
function bindCalendarEvents() {
    document.querySelectorAll('.calendar-day:not(.empty)').forEach(day => {
        day.addEventListener('click', function () {
            selectDate(parseInt(this.getAttribute('data-date')));
        });
    });
}

// ✅ Add these missing modal handlers!
function openSearchModal() {
    const modal = document.getElementById('searchModal');
    if (modal) modal.style.display = 'block';
}

function closeSearchModal() {
    const modal = document.getElementById('searchModal');
    if (modal) modal.style.display = 'none';
}

function openFilterModal() {
    const modal = document.getElementById('filterModal');
    if (modal) modal.style.display = 'block';
}

function closeFilterModal() {
    const modal = document.getElementById('filterModal');
    if (modal) modal.style.display = 'none';
}

function openSalesModal() {
    const modal = document.getElementById('salesModal');
    if (modal) modal.style.display = 'block';
}

function closeSalesModal() {
    const modal = document.getElementById('salesModal');
    if (modal) modal.style.display = 'none';
}

function clearFilters() {
    document.getElementById('stylistFilter').value = '';
    document.getElementById('typeFilter').value = '';
    document.getElementById('serviceFilter').value = '';
}

function applyFilters() {
    const stylist = document.getElementById('stylistFilter').value;
    const type = document.getElementById('typeFilter').value;
    const service = document.getElementById('serviceFilter').value;

    const filters = {};
    if (stylist) filters.stylist = stylist;
    if (type) filters.type = type;
    if (service) filters.service = service;

    const isoDate = formatDateToISO(currentYear, currentMonth, selectedDate);
    loadAppointments(isoDate, filters);
}

function clearSearch() {
    document.getElementById('searchInput').value = '';
}

function applySearch() {
    const query = document.getElementById('searchInput').value.trim();
    const filters = {};
    if (query) filters.search = query;

    const isoDate = formatDateToISO(currentYear, currentMonth, selectedDate);
    loadAppointments(isoDate, filters);
}

// Bind Modal Events
function bindModalEvents() {
    const actions = [
        ['prevMonthBtn', previousMonth],
        ['nextMonthBtn', nextMonth],
        ['searchBtn', openSearchModal],
        ['filterBtn', openFilterModal],
        ['salesOverviewCard', openSalesModal],
        ['addAppointmentBtn', () => openAddAppointmentModal(selectedDate)],
        ['cancelButton', closeAddAppointmentModal],
        ['searchClose', closeSearchModal],
        ['filterClose', closeFilterModal],
        ['salesClose', closeSalesModal],
        ['clearFilterBtn', clearFilters],
        ['applyFilterBtn', applyFilters]
    ];

    actions.forEach(([id, handler]) => {
        const el = document.getElementById(id);
        if (el) el.addEventListener('click', handler);
    });
}

// Populate Service Dropdown
async function populateServiceDropdown() {
    const serviceDropdown = document.getElementById('serviceType');
    serviceDropdown.innerHTML = '';

    try {
        const res = await fetch('/api/admin/services');
        const services = await res.json();
        if (services.length === 0) {
            serviceDropdown.innerHTML = '<option>No services available</option>';
            return;
        }

        services.forEach(service => {
            const option = document.createElement('option');
            option.value = service.serviceId;
            option.textContent = service.serviceName;
            serviceDropdown.appendChild(option);
        });
    } catch (err) {
        console.error('Error loading services:', err);
    }
}

// Populate Stylist Dropdown
async function populateStylistDropdown() {
    const stylistDropdown = document.getElementById('stylistName');
    stylistDropdown.innerHTML = '';

    try {
        const res = await fetch('/api/staff');
        const staffList = await res.json();
        if (staffList.length === 0) {
            stylistDropdown.innerHTML = '<option>No stylists available</option>';
            return;
        }

        staffList.forEach(staff => {
            const option = document.createElement('option');
            option.value = staff.staffId;
            option.textContent = staff.name;
            stylistDropdown.appendChild(option);
        });
    } catch (err) {
        console.error('Error loading stylists:', err);
    }
}

// Create Appointment API Call
async function createAppointment(data) {
    try {
        const res = await fetch('/api/appointments/create', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                CustomerId: data.customerName,
                BookingDate: data.appointmentDate,
                PreferredTime: data.appointmentTime,
                SelectedServices: [
                    {
                        ServiceId: data.serviceType,
                        PreferredStaffId: data.stylistName,
                        Price: 500,
                        DurationMins: 60
                    }
                ]
            })
        });

        if (!res.ok) throw new Error('Failed to create appointment');
        alert('Appointment created successfully!');
        closeAddAppointmentModal();
        loadAppointments(data.appointmentDate);
    } catch (err) {
        alert('Error creating appointment.');
        console.error(err);
    }
}

// Load Appointments
async function loadAppointments(dateStr, filters = {}) {
    const appointmentList = document.getElementById('appointmentList');
    const title = document.getElementById('selectedDateTitle');
    appointmentList.innerHTML = '<p>Loading...</p>';

    try {
        const params = new URLSearchParams({ date: dateStr, ...filters });
        const res = await fetch(`/api/appointments/day?${params.toString()}`);
        const appointments = await res.json();

        title.textContent = `Appointments for ${new Date(dateStr).toDateString()}`;
        renderAppointmentList(appointments);
    } catch (err) {
        console.error('Error fetching appointments:', err);
        appointmentList.innerHTML = '<p class="text-danger">Failed to load appointments.</p>';
    }
}

// Render Appointment List
function renderAppointmentList(appointments) {
    const appointmentList = document.getElementById('appointmentList');
    appointmentList.innerHTML = '';

    if (!appointments || appointments.length === 0) {
        appointmentList.innerHTML = '<p class="text-muted">No appointments for this date.</p>';
        return;
    }

    appointments.forEach(app => {
        const div = document.createElement('div');
        div.className = 'appointment-card';
        div.innerHTML = `
            <div class="appointment-header">
                <h5>${app.customerName} (${app.customerPhone})</h5>
                <span class="badge bg-${getStatusColor(app.status)}">${app.status}</span>
            </div>
            <p><strong>Service:</strong> ${app.serviceName}</p>
            <p><strong>Stylist:</strong> ${app.stylistName}</p>
            <p><strong>Time:</strong> ${formatTime(app.appointmentTime)}</p>
            <p><strong>Type:</strong> ${app.type}</p>
            <div class="btn-group">
                <button class="btn btn-sm btn-danger" onclick="cancelAppointment(${app.id})">Cancel</button>
                <button class="btn btn-sm btn-success" onclick="completeAppointment(${app.id})">Complete</button>
            </div>
        `;
        appointmentList.appendChild(div);
    });
}

// Format Time to 12-Hour
function formatTime(timeStr) {
    if (!timeStr || typeof timeStr !== 'string' || !timeStr.includes(':')) return 'N/A';
    const [hour, min] = timeStr.split(":");
    const h = parseInt(hour);
    const ampm = h >= 12 ? 'PM' : 'AM';
    return `${h % 12 || 12}:${min} ${ampm}`;
}

// Status Color
function getStatusColor(status) {
    if (!status) return 'info';
    switch (status.toLowerCase()) {
        case 'confirmed': return 'success';
        case 'pending': return 'warning';
        case 'cancelled': return 'secondary';
        default: return 'info';
    }
}

// Initialize Everything
initCurrentDate();

document.addEventListener('DOMContentLoaded', () => {
    updateCalendarTitle();
    selectDate(selectedDate);
    bindCalendarEvents();
    bindModalEvents();

    // ✅ Safe: bind form submit after DOM is ready
    const form = document.getElementById('addAppointmentForm');
    if (form) {
        form.addEventListener('submit', async function (e) {
            e.preventDefault();
            const data = {
                customerName: document.getElementById('customerName').value,
                customerPhone: document.getElementById('customerPhone').value,
                serviceType: document.getElementById('serviceType').value,
                stylistName: document.getElementById('stylistName').value,
                appointmentDate: document.getElementById('appointmentDate').value,
                appointmentTime: document.getElementById('appointmentTime').value,
                appointmentType: document.getElementById('appointmentType').value
            };
            await createAppointment(data);
        });
    }
});

// Handle Outside Clicks
window.onclick = handleOutsideModalClicks;
