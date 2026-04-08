<template>
    <div class="min-h-screen bg-gray-50 flex items-center justify-center p-4">
        <div class="text-center max-w-md">

            <!-- Visual -->
            <div class="relative mb-8 inline-block">
                <div class="text-9xl font-black text-gray-100 select-none leading-none">404</div>
                <div class="absolute inset-0 flex items-center justify-center">
                    <div class="text-5xl">🔍</div>
                </div>
            </div>

            <h1 class="text-2xl font-bold text-gray-800 mb-2">Page Not Found</h1>
            <p class="text-gray-500 mb-8">
                The page you're looking for doesn't exist or you don't have access to it.
            </p>

            <div class="flex gap-3 justify-center">
                <button @click="goBack"
                        class="px-5 py-2.5 border border-gray-300 text-gray-600 rounded-xl text-sm
          font-medium hover:bg-gray-100 transition-colors">
                    ← Go Back
                </button>
                <button @click="goHome"
                        class="px-5 py-2.5 bg-primary-600 hover:bg-primary-700 text-white rounded-xl
          text-sm font-medium transition-colors">
                    Go to Dashboard
                </button>
            </div>
        </div>
    </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router    = useRouter()
const authStore = useAuthStore()

function goBack() { router.back() }

function goHome() {
  const role = authStore.userRole
  if (role === 'Admin')      return router.push('/admin/dashboard')
  if (role === 'Instructor') return router.push('/instructor/dashboard')
  if (role === 'Student')    return router.push('/student/dashboard')
  router.push('/login')
}
</script>