<template>
    <div class="min-h-screen bg-gray-50 flex items-center justify-center">
        <div class="text-center">
            <h1 class="text-2xl font-bold text-gray-800">Admin Dashboard</h1>
            <p class="text-gray-500 mt-2">Welcome, {{ user?.fullName }}</p>
            <button @click="logout" class="mt-4 bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600">
                Logout
            </button>
        </div>
    </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/authStore'

const router    = useRouter()
const authStore = useAuthStore()
const user      = authStore.user

function logout() {
  authStore.logout()
  router.push('/login')
}
</script>