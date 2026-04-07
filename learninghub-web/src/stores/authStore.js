import { defineStore } from 'pinia'
import { authService } from '../services/authService'

export const useAuthStore = defineStore('auth', {
    state: () => ({
        user: JSON.parse(localStorage.getItem('user')) || null,
        token: localStorage.getItem('token') || null,
    }),

    getters: {
        isLoggedIn: state => !!state.token,
        isAdmin: state => state.user?.role === 'Admin',
        isInstructor: state => state.user?.role === 'Instructor',
        isStudent: state => state.user?.role === 'Student',
        userRole: state => state.user?.role || null
    },

    actions: {
        async login(email, password) {
            const data = await authService.login(email, password)
            this.token = data.token
            this.user = {
                id: data.userId,
                fullName: data.fullName,
                email: data.email,
                role: data.role
            }
            localStorage.setItem('token', this.token)
            localStorage.setItem('user', JSON.stringify(this.user))
        },

        async register(fullName, email, password, role) {
            const data = await authService.register(fullName, email, password, role)
            this.token = data.token
            this.user = {
                id: data.userId,
                fullName: data.fullName,
                email: data.email,
                role: data.role
            }
            localStorage.setItem('token', this.token)
            localStorage.setItem('user', JSON.stringify(this.user))
        },

        logout() {
            this.token = null
            this.user = null
            localStorage.removeItem('token')
            localStorage.removeItem('user')
        }
    }
})