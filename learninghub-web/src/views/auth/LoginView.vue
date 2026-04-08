<template>
    <div class="min-h-screen bg-gradient-to-br from-slate-900 via-primary-900 to-slate-900
    flex items-center justify-center p-4">

        <!-- Background decoration -->
        <div class="absolute inset-0 overflow-hidden pointer-events-none">
            <div class="absolute -top-40 -right-40 w-96 h-96 bg-primary-500 rounded-full
        opacity-10 blur-3xl" />
            <div class="absolute -bottom-40 -left-40 w-96 h-96 bg-blue-500 rounded-full
        opacity-10 blur-3xl" />
        </div>

        <div class="relative bg-white rounded-2xl shadow-2xl w-full max-w-md p-8">

            <!-- Logo -->
            <div class="text-center mb-8">
                <div class="inline-flex items-center justify-center w-14 h-14
          bg-primary-600 rounded-2xl mb-4 shadow-lg">
                    <span class="text-white text-2xl font-black">L</span>
                </div>
                <h1 class="text-2xl font-black text-gray-900">LearningHub</h1>
                <p class="text-gray-400 text-sm mt-1">Sign in to continue learning</p>
            </div>

            <!-- Error -->
            <div v-if="error"
                 class="bg-red-50 border border-red-200 text-red-700 rounded-xl
        px-4 py-3 mb-5 text-sm flex items-center gap-2">
                <span>⚠️</span> {{ error }}
            </div>

            <!-- Form -->
            <div class="space-y-4">
                <div>
                    <label class="block text-sm font-semibold text-gray-700 mb-1.5">
                        Email address
                    </label>
                    <input v-model="email"
                           type="email"
                           placeholder="you@example.com"
                           @keyup.enter="handleLogin"
                           class="w-full border border-gray-200 bg-gray-50 rounded-xl px-4 py-3
            text-sm focus:outline-none focus:ring-2 focus:ring-primary-500
            focus:bg-white focus:border-transparent transition-all" />
                </div>

                <div>
                    <label class="block text-sm font-semibold text-gray-700 mb-1.5">
                        Password
                    </label>
                    <input v-model="password"
                           type="password"
                           placeholder="••••••••"
                           @keyup.enter="handleLogin"
                           class="w-full border border-gray-200 bg-gray-50 rounded-xl px-4 py-3
            text-sm focus:outline-none focus:ring-2 focus:ring-primary-500
            focus:bg-white focus:border-transparent transition-all" />
                </div>

                <button @click="handleLogin"
                        :disabled="loading"
                        class="w-full bg-primary-600 hover:bg-primary-700 active:bg-primary-800
          text-white font-semibold py-3 rounded-xl transition-all
          disabled:opacity-60 disabled:cursor-not-allowed shadow-sm
          hover:shadow-primary-200 hover:shadow-lg">
                    {{ loading ? 'Signing in...' : 'Sign In' }}
                </button>
            </div>

            <!-- Divider -->
            <div class="flex items-center gap-3 my-6">
                <div class="flex-1 h-px bg-gray-100" />
                <span class="text-xs text-gray-400">Don't have an account?</span>
                <div class="flex-1 h-px bg-gray-100" />
            </div>

            <RouterLink to="/register"
                        class="block w-full text-center border border-gray-200 hover:border-primary-300
        hover:bg-primary-50 text-gray-600 hover:text-primary-700 font-semibold py-3
        rounded-xl transition-all text-sm">
                Create Account
            </RouterLink>
        </div>
    </div>
</template>

<script setup>
    import { ref } from 'vue'
    import { useRouter } from 'vue-router'
    import { useAuthStore } from '../../stores/authStore'

    const router = useRouter()
    const authStore = useAuthStore()

    const email = ref('')
    const password = ref('')
    const loading = ref(false)
    const error = ref('')

    async function handleLogin() {
        error.value = ''
        loading.value = true
        try {
            await authStore.login(email.value, password.value)
            redirectByRole(authStore.userRole)
        } catch (err) {
            error.value = err.response?.data?.message || 'Invalid email or password.'
        } finally {
            loading.value = false
        }
    }

    function redirectByRole(role) {
        if (role === 'Admin') return router.push('/admin/dashboard')
        if (role === 'Instructor') return router.push('/instructor/dashboard')
        if (role === 'Student') return router.push('/student/dashboard')
    }
</script>