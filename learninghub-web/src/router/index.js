import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = createRouter({
    history: createWebHistory(),
    routes: [
        { path: '/', redirect: '/login' },

        // ── Auth routes (public) ───────────────────────────────
        {
            path: '/login',
            name: 'Login',
            component: () => import('../views/auth/LoginView.vue'),
            meta: { guestOnly: true }
        },
        {
            path: '/register',
            name: 'Register',
            component: () => import('../views/auth/RegisterView.vue'),
            meta: { guestOnly: true }
        },

        // ── Admin routes ───────────────────────────────────────
        {
            path: '/admin/dashboard',
            name: 'AdminDashboard',
            component: () => import('../views/admin/DashboardView.vue'),
            meta: { requiresAuth: true, role: 'Admin' }
        },

        // ── Instructor routes ──────────────────────────────────
        {
            path: '/instructor/dashboard',
            name: 'InstructorDashboard',
            component: () => import('../views/instructor/DashboardView.vue'),
            meta: { requiresAuth: true, role: 'Instructor' }
        },

        // ── Student routes ─────────────────────────────────────
        {
            path: '/student/dashboard',
            name: 'StudentDashboard',
            component: () => import('../views/student/DashboardView.vue'),
            meta: { requiresAuth: true, role: 'Student' }
        },

        // ── 404 fallback ───────────────────────────────────────
        {
            path: '/:pathMatch(.*)*',
            redirect: '/login'
        }
    ]
})

// ── Route guard ────────────────────────────────────────────
router.beforeEach((to, from, next) => {
    const authStore = useAuthStore()

    // Redirect logged-in users away from login/register
    if (to.meta.guestOnly && authStore.isLoggedIn) {
        const role = authStore.userRole
        if (role === 'Admin') return next('/admin/dashboard')
        if (role === 'Instructor') return next('/instructor/dashboard')
        if (role === 'Student') return next('/student/dashboard')
    }

    // Redirect unauthenticated users to login
    if (to.meta.requiresAuth && !authStore.isLoggedIn) {
        return next('/login')
    }

    // Block wrong-role access
    if (to.meta.role && authStore.userRole !== to.meta.role) {
        const role = authStore.userRole
        if (role === 'Admin') return next('/admin/dashboard')
        if (role === 'Instructor') return next('/instructor/dashboard')
        if (role === 'Student') return next('/student/dashboard')
        return next('/login')
    }

    next()
})

export default router