// coupons.js
document.addEventListener("DOMContentLoaded", function () {
    const couponForm = document.getElementById("coupon-form");
    const couponTbody = document.getElementById("coupon-tbody");
    const searchInput = document.querySelector('.search-input');
    const statusSelect = document.getElementById('coupon-status-filter');
    const typeSelect = document.getElementById('coupon-type-filter');



    searchInput.addEventListener('input', filterCoupons);
    statusSelect.addEventListener('change', filterCoupons);
    typeSelect.addEventListener('change', filterCoupons);



    // Load all coupons on page load
    loadCoupons();
    

    async function loadCoupons() {
        try {
            const response = await fetch("/api/coupons");
            const data = await response.json();
            couponTbody.innerHTML = "";
            data.forEach(coupon => {
                const row = document.createElement("tr");
                row.innerHTML = `
  <td><strong>${coupon.Code}</strong></td>
  <td>${coupon.Description}</td>
  <td>${coupon.IsPercentage ? "Percentage" : "Fixed"}</td>
  <td>${coupon.DiscountAmount}${coupon.IsPercentage ? "%" : ""}</td>
  <td>${coupon.IsActive ? "Active" : "Inactive"}</td>
  <td>
    <button onclick="editCoupon('${coupon.CouponId}')">Edit</button>
    <button onclick="deleteCoupon('${coupon.CouponId}')">Delete</button>
  </td>
`;

                couponTbody.appendChild(row);
            });
        } catch (error) {
            console.error("Failed to load coupons:", error);
        }
    }
    filterCoupons();

    window.editCoupon = async function (id) {
        try {
            const response = await fetch(`/api/coupons/${id}`);
            const coupon = await response.json();
            document.getElementById("coupon-modal-title").textContent = "Edit Coupon";
           // couponForm.setAttribute("data-id", id);
            document.getElementById("coupon-code").value = coupon.Code;
            document.getElementById("coupon-name").value = coupon.Description;
            document.getElementById("coupon-type").value = coupon.IsPercentage ? "PERCENTAGE" : "FIXED";
            document.getElementById("coupon-discount").value = coupon.DiscountAmount;
            openModal("coupon-modal");
        } catch (error) {
            console.error("Failed to load coupon:", error);
        }
    };

    window.deleteCoupon = async function (id) {
        if (confirm("Are you sure you want to delete this coupon?")) {
            try {
                await fetch(`/api/coupons/${id}`, { method: "DELETE" });
                alert("Coupon deleted successfully.");
                loadCoupons();
            } catch (error) {
                console.error("Failed to delete coupon:", error);
            }
        }
    };

    couponForm.addEventListener("submit", async function (e) {
        e.preventDefault();
        const id = couponForm.getAttribute("data-id");
        const dto = {
            Code: document.getElementById("coupon-code").value.trim(),
            Description: document.getElementById("coupon-name").value.trim(),
            IsPercentage: document.getElementById("coupon-type").value === "PERCENTAGE",
            DiscountAmount: parseFloat(document.getElementById("coupon-discount").value),
            IsActive: true
        };

        try {
            if (id) {
                await fetch(`/api/coupons/${id}`, {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(dto)
                });
                alert("Coupon updated successfully.");
            } else {
                await fetch("/api/coupons", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(dto)
                });
                alert("Coupon created successfully.");
            }
            couponForm.removeAttribute("data-id");
            closeModal();
            loadCoupons();
        } catch (error) {
            console.error("Failed to save coupon:", error);
        }
    });

    // Utility to open modal (reuses your HTML logic)
    window.openCouponModal = function () {
        couponForm.removeAttribute("data-id");
        document.getElementById("coupon-modal-title").textContent = "Add Coupon";
        openModal("coupon-modal");
    };

    window.closeModal = function () {
        document.getElementById('modal-overlay').style.display = 'none';
        document.querySelectorAll('.modal').forEach(m => m.style.display = 'none');
    };

    function openModal(modalId) {
        document.getElementById('modal-overlay').style.display = 'block';
        document.getElementById(modalId).style.display = 'block';
    }
    function filterCoupons() {
        const searchTerm = searchInput.value.toLowerCase();
        const statusValue = statusSelect.value;
        const typeValue = typeSelect.value;

        const rows = couponTbody.querySelectorAll('tr');
        rows.forEach(row => {
            const code = row.cells[0].textContent.toLowerCase();
            const name = row.cells[1].textContent.toLowerCase();
            const type = row.cells[2].textContent.trim();
            const status = row.cells[4].textContent.trim();

            const matchesSearch = code.includes(searchTerm) || name.includes(searchTerm);
            const matchesStatus = statusValue === 'All' || status.toLowerCase() === statusValue.toLowerCase();
            const matchesType = typeValue === 'All' || type.toLowerCase() === typeValue.toLowerCase();

            if (matchesSearch && matchesStatus && matchesType) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    }

});
