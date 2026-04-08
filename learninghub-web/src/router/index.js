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
            meta: { guestOnly: true, title: 'Sign In' }
        },
        {
            path: '/register',
            name: 'Register',
            component: () => import('../views/auth/RegisterView.vue'),
            meta: { guestOnly: true, title: 'Sign Up' }
        },
        {
            path: '/profile',
            name: 'Profile',
            component: () => import('../views/shared/ProfileView.vue'),
            meta: { requiresAuth: true, title: 'My Profile' }
        },

        // ── Admin routes ───────────────────────────────────────
        {
            path: '/admin/dashboard',
            name: 'AdminDashboard',
            component: () => import('../views/admin/DashboardView.vue'),
            meta: { requiresAuth: true, role: 'Admin', title: 'Admin Dashboard' }
        },

        // ── Instructor routes ──────────────────────────────────
        {
            path: '/instructor/dashboard',
            name: 'InstructorDashboard',
            component: () => import('../views/instructor/DashboardView.vue'),
            meta: { requiresAuth: true, role: 'Instructor', title: 'Instructor Dashboard' }
        },
        {
            path: '/instructor/courses/:courseId/content',
            name: 'CourseContent',
            component: () => import('../views/instructor/CourseContentView.vue'),
            meta: { requiresAuth: true, role: 'Instructor', title: 'Course Content' }
        },
        {
            path: '/instructor/courses/:courseId/progress',
            name: 'StudentProgress',
            component: () => import('../views/instructor/StudentProgressView.vue'),
            meta: { requiresAuth: true, role: 'Instructor', title: 'Student Progress' }
        },

        // ── Student routes ─────────────────────────────────────
        {
            path: '/student/dashboard',
            name: 'StudentDashboard',
            component: () => import('../views/student/DashboardView.vue'),
            meta: { requiresAuth: true, role: 'Student', title: 'My Dashboard' }
        },
        {
            path: '/student/courses/:courseId',
            name: 'StudentCourse',
            component: () => import('../views/student/CourseView.vue'),
            meta: { requiresAuth: true, role: 'Student', title: 'Course Details' }
        },

        // ── Shared routes (discussions & announcements) ────────
        {
            path: '/courses/:courseId/discussions',
            name: 'Discussions',
            component: () => import('../views/shared/DiscussionView.vue'),
            meta: { requiresAuth: true, title: 'Discussions' }
        },
        {
            path: '/courses/:courseId/announcements',
            name: 'Announcements',
            component: () => import('../views/shared/AnnouncementsView.vue'),
            meta: { requiresAuth: true, title: 'Announcements' }
        },

        // ── 404 fallback ───────────────────────────────────────
        {
            path: '/:pathMatch(.*)*',
            name: 'NotFound',
            component: () => import('../views/NotFoundView.vue'),
            meta: { title: 'Page Not Found' }
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

// ── Update page title after each navigation ────────────────
router.afterEach((to) => {
    const title = to.meta?.title
    document.title = title ? `${title} — LearningHub` : 'LearningHub'
})

export default router