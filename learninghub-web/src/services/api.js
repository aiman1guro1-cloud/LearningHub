import axios from 'axios'

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL
})

// Attach token to every request automatically
api.interceptors.request.use(config => {
    const token = localStorage.getItem('token')
    if (token) {
        config.headers.Authorization = `Bearer ${token}`
    }
    return config
})

// Global response error handler
api.interceptors.response.use(
    response => response,
    error => {
        const status = error.response?.status

        if (status === 401) {
            localStorage.removeItem('token')
            localStorage.removeItem('user')
            window.location.href = '/login'
        }

        if (status === 500) {
            // Dynamically import toast to avoid circular deps
            import('../stores/toastStore').then(({ useToastStore }) => {
                const toast = useToastStore()
                toast.error('A server error occurred. Please try again.')
            })
        }

        return Promise.reject(error)
    }
)

export default api