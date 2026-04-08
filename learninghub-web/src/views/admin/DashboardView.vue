<template>
    <div class="min-h-screen bg-gray-50">

        <!-- Navbar -->
        <nav class="bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
            <div class="flex items-center gap-3">
                <div class="w-8 h-8 bg-primary-600 rounded-lg flex items-center justify-center">
                    <span class="text-white font-bold text-sm">L</span>
                </div>
                <span class="font-semibold text-gray-800">LearningHub</span>
                <span class="text-gray-300">|</span>
                <span class="text-sm text-gray-500">Admin Panel</span>
            </div>
            <div class="flex items-center gap-4">
                <RouterLink :to="`/profile`"
                            class="text-sm text-gray-600 hover:text-primary-600 transition-colors cursor-pointer">
                    {{ user?.fullName }}
                </RouterLink>
                <button @click="logout"
                        class="text-sm text-red-500 hover:text-red-700 font-medium">
                    Logout
                </button>
            </div>
        </nav>

        <!-- Tab Navigation -->
        <div class="bg-white border-b border-gray-200 px-6">
            <div class="flex gap-6 max-w-6xl mx-auto">
                <button v-for="tab in tabs" :key="tab.key"
                        @click="activeTab = tab.key"
                        :class="activeTab === tab.key
            ? 'border-b-2 border-primary-600 text-primary-600 font-semibold'
            : 'text-gray-500 hover:text-gray-700'"
                        class="py-3 text-sm transition-colors">
                    {{ tab.label }}
                </button>
            </div>
        </div>

        <div class="max-w-6xl mx-auto px-6 py-8">

            <!-- ── Overview Tab ───────────────────────────────── -->
            <div v-if="activeTab === 'overview'">
                <div class="mb-6">
                    <h1 class="text-2xl font-bold text-gray-800">System Overview</h1>
                    <p class="text-gray-500 text-sm mt-1">Platform-wide statistics</p>
                </div>

                <div v-if="loadingStats" class="text-center py-16 text-gray-400">
                    Loading stats...
                </div>

                <div v-else>
                    <!-- Stats Grid -->
                    <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-4 mb-8">
                        <div class="bg-white rounded-xl border border-gray-200 p-4 text-center">
                            <p class="text-3xl font-bold text-primary-600">{{ stats.totalUsers }}</p>
                            <p class="text-xs text-gray-500 mt-1">Total Users</p>
                        </div>
                        <div class="bg-white rounded-xl border border-gray-200 p-4 text-center">
                            <p class="text-3xl font-bold text-blue-500">{{ stats.totalStudents }}</p>
                            <p class="text-xs text-gray-500 mt-1">Students</p>
                        </div>
                        <div class="bg-white rounded-xl border border-gray-200 p-4 text-center">
                            <p class="text-3xl font-bold text-green-500">{{ stats.totalInstructors }}</p>
                            <p class="text-xs text-gray-500 mt-1">Instructors</p>
                        </div>
                        <div class="bg-white rounded-xl border border-gray-200 p-4 text-center">
                            <p class="text-3xl font-bold text-amber-500">{{ stats.totalCourses }}</p>
                            <p class="text-xs text-gray-500 mt-1">Courses</p>
                        </div>
                        <div class="bg-white rounded-xl border border-gray-200 p-4 text-center">
                            <p class="text-3xl font-bold text-purple-500">{{ stats.totalEnrollments }}</p>
                            <p class="text-xs text-gray-500 mt-1">Enrollments</p>
                        </div>
                    </div>

                    <!-- Secondary Stats -->
                    <div class="grid grid-cols-2 sm:grid-cols-4 gap-4">
                        <div class="bg-white rounded-xl border border-gray-200 p-4">
                            <div class="flex items-center justify-between mb-2">
                                <p class="text-sm text-gray-500">Published Courses</p>
                                <span class="text-xs bg-green-100 text-green-700 px-2 py-0.5 rounded-full font-medium">
                                    Live
                                </span>
                            </div>
                            <p class="text-2xl font-bold text-gray-800">{{ stats.publishedCourses }}</p>
                        </div>
                        <div class="bg-white rounded-xl border border-gray-200 p-4">
                            <div class="flex items-center justify-between mb-2">
                                <p class="text-sm text-gray-500">Draft Courses</p>
                                <span class="text-xs bg-yellow-100 text-yellow-700 px-2 py-0.5 rounded-full font-medium">
                                    Draft
                                </span>
                            </div>
                            <p class="text-2xl font-bold text-gray-800">{{ stats.draftCourses }}</p>
                        </div>
                        <div class="bg-white rounded-xl border border-gray-200 p-4">
                            <p class="text-sm text-gray-500 mb-2">Total Materials</p>
                            <p class="text-2xl font-bold text-gray-800">{{ stats.totalMaterials }}</p>
                        </div>
                        <div class="bg-white rounded-xl border border-gray-200 p-4">
                            <p class="text-sm text-gray-500 mb-2">Discussion Posts</p>
                            <p class="text-2xl font-bold text-gray-800">{{ stats.totalDiscussionPosts }}</p>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ── Users Tab ──────────────────────────────────── -->
            <div v-if="activeTab === 'users'">
                <div class="flex items-center justify-between mb-6">
                    <div>
                        <h1 class="text-2xl font-bold text-gray-800">User Management</h1>
                        <p class="text-gray-500 text-sm mt-1">
                            {{ filteredUsers.length }} user{{ filteredUsers.length !== 1 ? 's' : '' }}
                        </p>
                    </div>
                    <button @click="openCreateUserModal"
                            class="bg-primary-600 hover:bg-primary-700 text-white px-4 py-2 rounded-lg text-sm font-medium transition-colors">
                        + Add User
                    </button>
                </div>

                <!-- Search & Filter -->
                <div class="flex gap-3 mb-4">
                    <input v-model="userSearch" type="text" placeholder="Search by name or email..."
                           class="flex-1 border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500" />
                    <select v-model="userRoleFilter"
                            class="border border-gray-300 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500">
                        <option value="">All Roles</option>
                        <option value="Admin">Admin</option>
                        <option value="Instructor">Instructor</option>
                        <option value="Student">Student</option>
                    </select>
                </div>

                <div v-if="loadingUsers" class="text-center py-16 text-gray-400">
                    Loading users...
                </div>

                <div v-else class="bg-white rounded-xl border border-gray-200 overflow-hidden">
                    <!-- Table Header -->
                    <div class="grid grid-cols-6 gap-4 px-5 py-3 bg-gray-50 border-b border-gray-200
            text-xs font-semibold text-gray-500 uppercase tracking-wide">
                        <div class="col-span-2">User</div>
                        <div>Role</div>
                        <div>Joined</div>
                        <div>Activity</div>
                        <div class="text-right">Actions</div>
                    </div>

                    <div v-if="filteredUsers.length === 0"
                         class="text-center py-12 text-gray-400">
                        No users found.
                    </div>

                    <div v-for="u in filteredUsers" :key="u.id"
                         class="grid grid-cols-6 gap-4 px-5 py-4 border-b border-gray-100 last:border-0
            items-center hover:bg-gray-50 transition-colors">

                        <div class="col-span-2">
                            <p class="text-sm font-semibold text-gray-800">{{ u.fullName }}</p>
                            <p class="text-xs text-gray-400 truncate">{{ u.email }}</p>
                        </div>

                        <div>
                            <span :class="{
                'bg-purple-100 text-purple-700': u.role === 'Admin',
                'bg-blue-100 text-blue-700':     u.role === 'Instructor',
                'bg-gray-100 text-gray-600':     u.role === 'Student'
              }" class="text-xs px-2 py-0.5 rounded-full font-medium">
                                {{ u.role }}
                            </span>
                        </div>

                        <div class="text-sm text-gray-500">
                            {{ formatDate(u.createdAt) }}
                        </div>

                        <div class="text-xs text-gray-400">
                            <span v-if="u.role === 'Instructor'">
                                {{ u.courseCount }} course{{ u.courseCount !== 1 ? 's' : '' }}
                            </span>
                            <span v-else-if="u.role === 'Student'">
                                {{ u.enrollmentCount }} enrolled
                            </span>
                            <span v-else>—</span>
                        </div>

                        <div class="flex gap-1 justify-end">
                            <button @click="openEditUserModal(u)"
                                    class="text-xs bg-gray-100 hover:bg-gray-200 text-gray-600 px-2 py-1 rounded-lg">
                                Edit
                            </button>
                            <button @click="openResetPasswordModal(u)"
                                    class="text-xs bg-amber-50 hover:bg-amber-100 text-amber-600 px-2 py-1 rounded-lg">
                                Reset PW
                            </button>
                            <button @click="confirmDeleteUser(u)"
                                    class="text-xs bg-red-50 hover:bg-red-100 text-red-500 px-2 py-1 rounded-lg">
                                Delete
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ── Courses Tab ─────────────────────────────────── -->
            <div v-if="activeTab === 'courses'">
                <div class="flex items-center justify-between mb-6">
                    <div>
                        <h1 class="text-2xl font-bold text-gray-800">All Courses</h1>
                        <p class="text-gray-500 text-sm mt-1">
                            {{ adminCourses.length }} course{{ adminCourses.length !== 1 ? 's' : '' }} total
                        </p>
                    </div>
                </div>

                <!-- Search -->
                <input v-model="courseSearch" type="text" placeholder="Search courses..."
                       class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm mb-4
          focus:outline-none focus:ring-2 focus:ring-primary-500" />

                <div v-if="loadingCourses" class="text-center py-16 text-gray-400">
                    Loading courses...
                </div>

                <div v-else class="bg-white rounded-xl border border-gray-200 overflow-hidden">
                    <div class="grid grid-cols-6 gap-4 px-5 py-3 bg-gray-50 border-b border-gray-200
            text-xs font-semibold text-gray-500 uppercase tracking-wide">
                        <div class="col-span-2">Course</div>
                        <div>Instructor</div>
                        <div>Status</div>
                        <div>Students</div>
                        <div class="text-right">Actions</div>
                    </div>

                    <div v-if="filteredAdminCourses.length === 0"
                         class="text-center py-12 text-gray-400">
                        No courses found.
                    </div>

                    <div v-for="course in filteredAdminCourses" :key="course.id"
                         class="grid grid-cols-6 gap-4 px-5 py-4 border-b border-gray-100 last:border-0
            items-center hover:bg-gray-50 transition-colors">

                        <div class="col-span-2">
                            <p class="text-sm font-semibold text-gray-800">{{ course.title }}</p>
                            <p class="text-xs text-gray-400 line-clamp-1">{{ course.description }}</p>
                        </div>

                        <div class="text-sm text-gray-600">{{ course.instructorName }}</div>

                        <div>
                            <span :class="course.isPublished
                ? 'bg-green-100 text-green-700'
                : 'bg-yellow-100 text-yellow-700'"
                                  class="text-xs px-2 py-0.5 rounded-full font-medium">
                                {{ course.isPublished ? 'Published' : 'Draft' }}
                            </span>
                        </div>

                        <div class="text-sm text-gray-500">
                            {{ course.enrollmentCount }}
                        </div>

                        <div class="flex justify-end">
                            <button @click="confirmDeleteCourse(course)"
                                    class="text-xs bg-red-50 hover:bg-red-100 text-red-500 px-2 py-1 rounded-lg">
                                Delete
                            </button>
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <!-- ── Create / Edit User Modal ─────────────────────── -->
        <div v-if="showUserModal"
             class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-md p-6">
                <h2 class="text-lg font-bold text-gray-800 mb-4">
                    {{ editingUser ? 'Edit User' : 'Add User' }}
                </h2>

                <div v-if="userModalError"
                     class="bg-red-50 text-red-600 text-sm rounded-lg px-4 py-2 mb-4">
                    {{ userModalError }}
                </div>

                <div class="space-y-4">
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Full Name</label>
                        <input v-model="userForm.fullName" type="text" placeholder="Full name"
                               class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm
              focus:outline-none focus:ring-2 focus:ring-primary-500" />
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Email</label>
                        <input v-model="userForm.email" type="email" placeholder="email@example.com"
                               class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm
              focus:outline-none focus:ring-2 focus:ring-primary-500" />
                    </div>
                    <div v-if="!editingUser">
                        <label class="block text-sm font-medium text-gray-700 mb-1">Password</label>
                        <input v-model="userForm.password" type="password" placeholder="Min. 6 characters"
                               class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm
              focus:outline-none focus:ring-2 focus:ring-primary-500" />
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Role</label>
                        <select v-model="userForm.role"
                                class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm
              focus:outline-none focus:ring-2 focus:ring-primary-500">
                            <option value="Student">Student</option>
                            <option value="Instructor">Instructor</option>
                            <option value="Admin">Admin</option>
                        </select>
                    </div>
                </div>

                <div class="flex justify-end gap-3 mt-6">
                    <button @click="showUserModal = false"
                            class="px-4 py-2 text-sm text-gray-600 font-medium hover:text-gray-800">
                        Cancel
                    </button>
                    <button @click="submitUser" :disabled="savingUser"
                            class="px-4 py-2 bg-primary-600 hover:bg-primary-700 text-white text-sm
            font-medium rounded-lg disabled:opacity-60 transition-colors">
                        {{ savingUser ? 'Saving...' : (editingUser ? 'Save Changes' : 'Create User') }}
                    </button>
                </div>
            </div>
        </div>

        <!-- ── Reset Password Modal ──────────────────────────── -->
        <div v-if="showResetPwModal"
             class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-sm p-6">
                <h2 class="text-lg font-bold text-gray-800 mb-1">Reset Password</h2>
                <p class="text-sm text-gray-500 mb-4">
                    For: <span class="font-medium">{{ resetPwUser?.fullName }}</span>
                </p>

                <div v-if="resetPwError"
                     class="bg-red-50 text-red-600 text-sm rounded-lg px-4 py-2 mb-4">
                    {{ resetPwError }}
                </div>

                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">New Password</label>
                    <input v-model="newPassword" type="password" placeholder="Min. 6 characters"
                           class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm
            focus:outline-none focus:ring-2 focus:ring-primary-500" />
                </div>

                <div v-if="resetPwSuccess"
                     class="mt-3 bg-green-50 text-green-700 text-sm rounded-lg px-4 py-2">
                    {{ resetPwSuccess }}
                </div>

                <div class="flex justify-end gap-3 mt-6">
                    <button @click="showResetPwModal = false"
                            class="px-4 py-2 text-sm text-gray-600 font-medium hover:text-gray-800">
                        Close
                    </button>
                    <button @click="submitResetPassword" :disabled="savingUser"
                            class="px-4 py-2 bg-amber-500 hover:bg-amber-600 text-white text-sm
            font-medium rounded-lg disabled:opacity-60 transition-colors">
                        {{ savingUser ? 'Resetting...' : 'Reset Password' }}
                    </button>
                </div>
            </div>
        </div>

        <!-- ── Delete Confirm Modal ──────────────────────────── -->
        <div v-if="showDeleteModal"
             class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-sm p-6 text-center">
                <div class="text-4xl mb-3">🗑️</div>
                <h2 class="text-lg font-bold text-gray-800 mb-2">
                    Delete {{ deleteType === 'user' ? 'User' : 'Course' }}?
                </h2>
                <p class="text-sm text-gray-500 mb-6">
                    "<span class="font-medium">{{ deletingItem?.fullName || deletingItem?.title }}</span>"
                    will be permanently deleted.
                </p>
                <div class="flex gap-3 justify-center">
                    <button @click="showDeleteModal = false"
                            class="px-4 py-2 text-sm border border-gray-300 rounded-lg hover:bg-gray-50">
                        Cancel
                    </button>
                    <button @click="executeDelete" :disabled="deleting"
                            class="px-4 py-2 bg-red-500 hover:bg-red-600 text-white text-sm
            rounded-lg disabled:opacity-60">
                        {{ deleting ? 'Deleting...' : 'Delete' }}
                    </button>
                </div>
            </div>
        </div>

    </div>
</template>

<script setup>
    import { ref, computed, onMounted } from 'vue'
    import { useRouter } from 'vue-router'
    import { useAuthStore } from '../../stores/authStore'
    import { useToastStore } from '../../stores/toastStore'
    import { adminService } from '../../services/adminService'

    const router = useRouter()
    const authStore = useAuthStore()
    const toast = useToastStore()
    const user = authStore.user

    // ── Tabs ─────────────────────────────────────────────────
    const tabs = [
        { key: 'overview', label: '📊 Overview' },
        { key: 'users', label: '👥 Users' },
        { key: 'courses', label: '📚 Courses' }
    ]
    const activeTab = ref('overview')

    // ── Stats ─────────────────────────────────────────────────
    const stats = ref({})
    const loadingStats = ref(true)

    // ── Users ─────────────────────────────────────────────────
    const users = ref([])
    const loadingUsers = ref(true)
    const userSearch = ref('')
    const userRoleFilter = ref('')

    // ── Courses ───────────────────────────────────────────────
    const adminCourses = ref([])
    const loadingCourses = ref(true)
    const courseSearch = ref('')

    // ── User Modal ────────────────────────────────────────────
    const showUserModal = ref(false)
    const editingUser = ref(null)
    const userForm = ref({ fullName: '', email: '', password: '', role: 'Student' })
    const userModalError = ref('')
    const savingUser = ref(false)

    // ── Reset Password Modal ──────────────────────────────────
    const showResetPwModal = ref(false)
    const resetPwUser = ref(null)
    const newPassword = ref('')
    const resetPwError = ref('')
    const resetPwSuccess = ref('')

    // ── Delete Modal ──────────────────────────────────────────
    const showDeleteModal = ref(false)
    const deletingItem = ref(null)
    const deleteType = ref('')
    const deleting = ref(false)

    // ── Computed ──────────────────────────────────────────────
    const filteredUsers = computed(() => {
        let list = users.value
        if (userRoleFilter.value)
            list = list.filter(u => u.role === userRoleFilter.value)
        if (userSearch.value.trim()) {
            const q = userSearch.value.toLowerCase()
            list = list.filter(u =>
                u.fullName.toLowerCase().includes(q) ||
                u.email.toLowerCase().includes(q)
            )
        }
        return list
    })

    const filteredAdminCourses = computed(() => {
        if (!courseSearch.value.trim()) return adminCourses.value
        const q = courseSearch.value.toLowerCase()
        return adminCourses.value.filter(c =>
            c.title.toLowerCase().includes(q) ||
            c.instructorName.toLowerCase().includes(q)
        )
    })

    // ── Lifecycle ─────────────────────────────────────────────
    onMounted(async () => {
        await Promise.all([loadStats(), loadUsers(), loadCourses()])
    })

    async function loadStats() {
        loadingStats.value = true
        try {
            const res = await adminService.getStats()
            stats.value = res.data
        } finally {
            loadingStats.value = false
        }
    }

    async function loadUsers() {
        loadingUsers.value = true
        try {
            const res = await adminService.getUsers()
            users.value = res.data
        } finally {
            loadingUsers.value = false
        }
    }

    async function loadCourses() {
        loadingCourses.value = true
        try {
            const res = await adminService.getAllCourses()
            adminCourses.value = res.data
        } finally {
            loadingCourses.value = false
        }
    }

    // ── User CRUD ─────────────────────────────────────────────
    function openCreateUserModal() {
        editingUser.value = null
        userForm.value = { fullName: '', email: '', password: '', role: 'Student' }
        userModalError.value = ''
        showUserModal.value = true
    }

    function openEditUserModal(u) {
        editingUser.value = u
        userForm.value = { fullName: u.fullName, email: u.email, password: '', role: u.role }
        userModalError.value = ''
        showUserModal.value = true
    }

    async function submitUser() {
        if (!userForm.value.fullName.trim() || !userForm.value.email.trim()) {
            userModalError.value = 'Name and email are required.'
            return
        }
        savingUser.value = true
        userModalError.value = ''
        try {
            if (editingUser.value) {
                await adminService.updateUser(editingUser.value.id, userForm.value)
            } else {
                if (!userForm.value.password || userForm.value.password.length < 6) {
                    userModalError.value = 'Password must be at least 6 characters.'
                    return
                }
                await adminService.createUser(userForm.value)
            }
            showUserModal.value = false
            await loadUsers()
            await loadStats()
        } catch (err) {
            userModalError.value = err.response?.data?.message || 'Something went wrong.'
        } finally {
            savingUser.value = false
        }
    }

    // ── Reset Password ────────────────────────────────────────
    function openResetPasswordModal(u) {
        resetPwUser.value = u
        newPassword.value = ''
        resetPwError.value = ''
        resetPwSuccess.value = ''
        showResetPwModal.value = true
    }

    async function submitResetPassword() {
        if (!newPassword.value || newPassword.value.length < 6) {
            resetPwError.value = 'Password must be at least 6 characters.'
            return
        }
        savingUser.value = true
        resetPwError.value = ''
        try {
            await adminService.resetPassword(resetPwUser.value.id, newPassword.value)
            resetPwSuccess.value = 'Password reset successfully.'
            newPassword.value = ''
        } catch (err) {
            resetPwError.value = err.response?.data?.message || 'Failed to reset password.'
        } finally {
            savingUser.value = false
        }
    }

    // ── Delete ────────────────────────────────────────────────
    function confirmDeleteUser(u) {
        deletingItem.value = u
        deleteType.value = 'user'
        showDeleteModal.value = true
    }

    function confirmDeleteCourse(course) {
        deletingItem.value = course
        deleteType.value = 'course'
        showDeleteModal.value = true
    }

    async function executeDelete() {
        deleting.value = true
        try {
            if (deleteType.value === 'user') {
                await adminService.deleteUser(deletingItem.value.id)
                await loadUsers()
                await loadStats()
            } else {
                await adminService.deleteCourse(deletingItem.value.id)
                await loadCourses()
                await loadStats()
            }
            showDeleteModal.value = false
        } catch (err) {
            toast.error(err.response?.data?.message || 'Delete failed.')
        } finally {
            deleting.value = false
        }
    }

    // ── Helpers ───────────────────────────────────────────────
    function formatDate(dateStr) {
        return new Date(dateStr).toLocaleDateString('en-US', {
            year: 'numeric', month: 'short', day: 'numeric'
        })
    }

    function logout() {
        authStore.logout()
        router.push('/login')
    }
</script>