<template>
    <div class="min-h-screen bg-gradient-to-br from-primary-50 to-blue-100 flex items-center justify-center p-4">
        <div class="bg-white rounded-2xl shadow-lg w-full max-w-md p-8">

            <!-- Logo -->
            <div class="text-center mb-8">
                <div class="inline-flex items-center justify-center w-14 h-14 bg-primary-600 rounded-xl mb-3">
                    <span class="text-white text-2xl font-bold">L</span>
                </div>
                <h1 class="text-2xl font-bold text-gray-800">Create Account</h1>
                <p class="text-gray-500 text-sm mt-1">Join LearningHub today</p>
            </div>

            <!-- Error -->
            <div v-if="error" class="bg-red-50 border border-red-200 text-red-700 rounded-lg px-4 py-3 mb-4 text-sm">
                {{ error }}
            </div>

            <!-- Form -->
            <form @submit.prevent="handleRegister" class="space-y-4">
                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Full Name</label>
                    <input v-model="fullName"
                           type="text"
                           placeholder="Juan dela Cruz"
                           class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
                           required />
                </div>

                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Email</label>
                    <input v-model="email"
                           type="email"
                           placeholder="you@example.com"
                           class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
                           required />
                </div>

                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Password</label>
                    <input v-model="password"
                           type="password"
                           placeholder="Min. 6 characters"
                           class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
                           required />
                </div>

                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Register as</label>
                    <select v-model="role"
                            class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent">
                        <option value="Student">Student</option>
                        <option value="Instructor">Instructor</option>
                    </select>
                </div>

                <button type="submit"
                        :disabled="loading"
                        class="w-full bg-primary-600 hover:bg-primary-700 text-white font-medium py-2.5 rounded-lg transition-colors disabled:opacity-60 disabled:cursor-not-allowed">
                    {{ loading ? 'Creating account...' : 'Create Account' }}
                </button>
            </form>

            <!-- Login link -->
            <p class="text-center text-sm text-gray-500 mt-6">
                Already have an account?
                <RouterLink to="/login" class="text-primary-600 font-medium hover:underline">Sign in</RouterLink>
            </p>
        </div>
    </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/authStore'

const router    = useRouter()
const authStore = useAuthStore()

const fullName = ref('')
const email    = ref('')
const password = ref('')
const role     = ref('Student')
const loading  = ref(false)
const error    = ref('')

async function handleRegister() {
  error.value   = ''
  loading.value = true
  try {
    await authStore.register(fullName.value, email.value, password.value, role.value)
    const userRole = authStore.userRole
    if (userRole === 'Instructor') return router.push('/instructor/dashboard')
    if (userRole === 'Student')    return router.push('/student/dashboard')
  } catch (err) {
    error.value = err.response?.data?.message || 'Registration failed. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>