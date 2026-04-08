<template>
    <div class="min-h-screen bg-gray-50">

        <!-- Navbar -->
        <nav class="bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
            <div class="flex items-center gap-3">
                <button @click="goBack"
                        class="text-gray-400 hover:text-gray-600 transition-colors">
                    ← Back
                </button>
                <span class="text-gray-300">|</span>
                <span class="font-semibold text-gray-800">My Profile</span>
            </div>
        </nav>

        <div class="max-w-xl mx-auto px-6 py-10">

            <!-- Avatar card -->
            <div class="bg-white rounded-2xl border border-gray-200 p-6 mb-6 text-center">
                <div class="w-20 h-20 rounded-2xl flex items-center justify-center text-3xl
          font-black text-white mx-auto mb-4"
                     :class="{
            'bg-purple-500': authStore.user?.role === 'Admin',
            'bg-blue-500':   authStore.user?.role === 'Instructor',
            'bg-primary-500': authStore.user?.role === 'Student'
          }">
                    {{ authStore.user?.fullName?.charAt(0)?.toUpperCase() }}
                </div>
                <h2 class="text-xl font-bold text-gray-800">{{ authStore.user?.fullName }}</h2>
                <p class="text-gray-500 text-sm mt-0.5">{{ authStore.user?.email }}</p>
                <span :class="{
          'bg-purple-100 text-purple-700': authStore.user?.role === 'Admin',
          'bg-blue-100 text-blue-700':     authStore.user?.role === 'Instructor',
          'bg-primary-100 text-primary-700': authStore.user?.role === 'Student'
        }" class="inline-block mt-2 text-xs px-3 py-1 rounded-full font-semibold">
                    {{ authStore.user?.role }}
                </span>
            </div>

            <!-- Change Password -->
            <div class="bg-white rounded-2xl border border-gray-200 p-6">
                <h3 class="text-base font-bold text-gray-800 mb-4">Change Password</h3>

                <div v-if="successMsg"
                     class="bg-green-50 border border-green-200 text-green-700 text-sm
          rounded-lg px-4 py-3 mb-4">
                    {{ successMsg }}
                </div>
                <div v-if="errorMsg"
                     class="bg-red-50 border border-red-200 text-red-600 text-sm
          rounded-lg px-4 py-3 mb-4">
                    {{ errorMsg }}
                </div>

                <div class="space-y-4">
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">
                            Current Password
                        </label>
                        <input v-model="form.currentPassword" type="password"
                               placeholder="Enter current password"
                               class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm
              focus:outline-none focus:ring-2 focus:ring-primary-500" />
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">
                            New Password
                        </label>
                        <input v-model="form.newPassword" type="password"
                               placeholder="Min. 6 characters"
                               class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm
              focus:outline-none focus:ring-2 focus:ring-primary-500" />
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">
                            Confirm New Password
                        </label>
                        <input v-model="form.confirmPassword" type="password"
                               placeholder="Repeat new password"
                               class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm
              focus:outline-none focus:ring-2 focus:ring-primary-500" />
                    </div>

                    <button @click="changePassword" :disabled="saving"
                            class="w-full bg-primary-600 hover:bg-primary-700 text-white font-medium
            py-2.5 rounded-lg text-sm transition-colors disabled:opacity-60">
                        {{ saving ? 'Saving...' : 'Update Password' }}
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/authStore'
import { useToastStore } from '../../stores/toastStore'
import api from '../../services/api'

const router    = useRouter()
const authStore = useAuthStore()
const toast     = useToastStore()

const form = ref({
  currentPassword: '',
  newPassword:     '',
  confirmPassword: ''
})
const saving     = ref(false)
const successMsg = ref('')
const errorMsg   = ref('')

async function changePassword() {
  successMsg.value = ''
  errorMsg.value   = ''

  if (!form.value.currentPassword || !form.value.newPassword) {
    errorMsg.value = 'All fields are required.'
    return
  }
  if (form.value.newPassword.length < 6) {
    errorMsg.value = 'New password must be at least 6 characters.'
    return
  }
  if (form.value.newPassword !== form.value.confirmPassword) {
    errorMsg.value = 'Passwords do not match.'
    return
  }

  saving.value = true
  try {
    await api.post('/auth/change-password', {
      currentPassword: form.value.currentPassword,
      newPassword:     form.value.newPassword
    })
    successMsg.value = 'Password updated successfully.'
    toast.success('Password updated!')
    form.value = { currentPassword: '', newPassword: '', confirmPassword: '' }
  } catch (err) {
    errorMsg.value = err.response?.data?.message || 'Failed to update password.'
  } finally {
    saving.value = false
  }
}

function goBack() { router.back() }
</script>