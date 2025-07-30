// ✅ Base API URL
const API_BASE = '/api/user';

// ✅ Global variables
let users = [];
//let editingUserId = null;
//let searchTerm = '';

// ✅ Fetch all users from backend
async function getAllUsers() {
    try {
        const res = await fetch(API_BASE);
        if (!res.ok) throw new Error('Failed to fetch users');
        return await res.json();
    } catch (err) {
        console.error(err);
        return [];
    }
}

// ✅ Add user via API
async function addUserAPI(userData) {
    try {
        const res = await fetch(API_BASE, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userData)
        });
        return await res.json();
    } catch (err) {
        console.error(err);
    }
}

// ✅ Update user via API
async function updateUserAPI(userId, userData) {
    try {
        const res = await fetch(`${API_BASE}/${userId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userData)
        });
        return await res.json();
    } catch (err) {
        console.error(err);
    }
}

// ✅ Deactivate user via API
async function deleteUserAPI(userId) {
    try {
        const res = await fetch(`${API_BASE}/${userId}`, {
            method: 'DELETE'
        });
        return await res.json();
    } catch (err) {
        console.error(err);
    }
}

// ✅ Initialize app
async function init() {
    users = await getAllUsers();
    renderUsers();
    updateStatistics();

    document.getElementById('searchInput').addEventListener('input', function (e) {
        searchTerm = e.target.value.toLowerCase();
        renderUsers();
    });
}

// ✅ Render users table
function renderUsers() {
    const tbody = document.getElementById('userTableBody');
    const emptyState = document.getElementById('emptyState');

    const filteredUsers = users.filter(user =>
        user.Username.toLowerCase().includes(searchTerm) ||
        user.RoleName.toLowerCase().includes(searchTerm)
    );

    if (filteredUsers.length === 0) {
        tbody.innerHTML = '';
        emptyState.style.display = 'block';
        return;
    }

    emptyState.style.display = 'none';

    tbody.innerHTML = filteredUsers.map(user => {
        const isActive = user.IsActive !== false; // default true if null

        if (editingUserId === user.UserId) {
            return `
        <tr>
          <td><input type="text" class="edit-input" value="${user.Username}" id="edit-username-${user.UserId}"></td>
          <td>
            <select class="edit-select" id="edit-role-${user.UserId}">
              <option value="Admin" ${user.RoleName === 'Admin' ? 'selected' : ''}>Admin</option>
              <option value="Manager" ${user.RoleName === 'Manager' ? 'selected' : ''}>Manager</option>
              <option value="Staff" ${user.RoleName === 'Staff' ? 'selected' : ''}>Staff</option>
              <!-- add other roles -->
            </select>
          </td>
          <td>
            <select class="edit-select" id="edit-active-${user.UserId}">
              <option value="true" ${isActive ? 'selected' : ''}>Active</option>
              <option value="false" ${!isActive ? 'selected' : ''}>Inactive</option>
            </select>
          </td>
          <td>
            <button onclick="saveUser('${user.UserId}')">Save</button>
            <button onclick="cancelEdit()">Cancel</button>
          </td>
        </tr>
      `;
        } else {
            return `
        <tr>
          <td>${user.Username}</td>
          <td>${user.RoleName}</td>
          <td>${isActive ? 'Active' : 'Inactive'}</td>
          <td>
            <button onclick="editUser('${user.UserId}')">Edit</button>
            <button onclick="deleteUser('${user.UserId}')">Delete</button>
          </td>
        </tr>
      `;
        }
    }).join('');
}

// ✅ Update stats cards
function updateStatistics() {
    document.getElementById('totalUsers').textContent = users.length;
    document.getElementById('adminUsers').textContent = users.filter(u => u.RoleName === 'Admin').length;
    document.getElementById('managerUsers').textContent = users.filter(u => u.RoleName === 'Manager').length;
    document.getElementById('staffUsers').textContent = users.filter(u => u.RoleName === 'Staff').length;
}

// ✅ Edit user
function editUser(userId) {
    editingUserId = userId;
    renderUsers();
}

// ✅ Save user update
async function saveUser(userId) {
    const username = document.getElementById(`edit-username-${userId}`).value.trim();
    const role = document.getElementById(`edit-role-${userId}`).value;
    const isActive = document.getElementById(`edit-active-${userId}`).value === 'true';

    const user = users.find(u => u.UserId === userId);
    const payload = {
        Username: username,
        Password: '', // only update if needed
        Role: role
    };

    await updateUserAPI(userId, payload);

    // locally update status
    user.Username = username;
    user.RoleName = role;
    user.IsActive = isActive;

    editingUserId = null;
    renderUsers();
    updateStatistics();
}

// ✅ Cancel edit
function cancelEdit() {
    editingUserId = null;
    renderUsers();
}

// ✅ Delete user
async function deleteUser(userId) {
    if (confirm('Are you sure you want to deactivate this user?')) {
        await deleteUserAPI(userId);
        users = users.filter(u => u.UserId !== userId);
        renderUsers();
        updateStatistics();
    }
}

// ✅ Add new user
function addUser() {
    document.getElementById("addUserModal").style.display = "flex";
}

function closeModal() {
    document.getElementById("addUserModal").style.display = "none";
    document.getElementById("newUsername").value = '';
    document.getElementById("newPassword").value = '';
    document.getElementById("newRole").value = 'Admin';
}

async function submitNewUser() {
    const username = document.getElementById("newUsername").value.trim();
    const password = document.getElementById("newPassword").value.trim();
    const role = document.getElementById("newRole").value;

    if (!username || !password || !role) {
        alert("Please fill all fields.");
        return;
    }

    const payload = { Username: username, Password: password, Role: role };

    const result = await addUserAPI(payload);
    if (result) {
        users = await getAllUsers(); // refresh
        renderUsers();
        updateStatistics();
        closeModal();
    } else {
        alert("Failed to add user.");
    }
}


// ✅ Start everything
document.addEventListener('DOMContentLoaded', init);
