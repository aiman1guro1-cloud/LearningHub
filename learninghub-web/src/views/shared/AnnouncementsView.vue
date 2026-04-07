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
                <span class="font-semibold text-gray-800 truncate max-w-xs">{{ courseName }}</span>
            </div>
            <span class="text-sm font-medium text-gray-500">Announcements</span>
        </nav>

        <div class="max-w-3xl mx-auto px-6 py-8">

            <div class="flex items-center justify-between mb-6">
                <div>
                    <h1 class="text-2xl font-bold text-gray-800">Announcements</h1>
                    <p class="text-gray-500 text-sm mt-1">Important updates from your instructor</p>
                </div>
                <button v-if="isInstructor" @click="openCreateModal"
                        class="bg-primary-600 hover:bg-primary-700 text-white px-4 py-2 rounded-lg text-sm font-medium transition-colors">
                    + New Announcement
                </button>
            </div>

            <div v-if="loading" class="text-center py-16 text-gray-400">Loading...</div>

            <div v-else-if="announcements.length === 0" class="text-center py-16">
                <div class="text-5xl mb-4">📢</div>
                <p class="text-gray-500">No announcements yet.</p>
            </div>

            <div v-else class="space-y-4">
                <div v-for="ann in announcements" :key="ann.id"
                     class="bg-white rounded-xl border border-gray-200 p-5 hover:shadow-sm transition-shadow">
                    <div class="flex items-start justify-between gap-4">
                        <div class="flex-1">
                            <div class="flex items-center gap-2 mb-2">
                                <span class="w-2 h-2 bg-primary-500 rounded-full shrink-0" />
                                <h3 class="font-semibold text-gray-800">{{ ann.title }}</h3>
                            </div>
                            <p class="text-sm text-gray-600 leading-relaxed mb-3">{{ ann.content }}</p>
                            <div class="flex items-center gap-2 text-xs text-gray-400">
                                <span class="bg-blue-100 text-blue-700 px-1.5 py-0.5 rounded font-medium">Instructor</span>
                                <span>{{ ann.instructorName }}</span>
                                <span>·</span>
                                <span>{{ formatDate(ann.createdAt) }}</span>
                            </div>
                        </div>

                        <!-- Instructor actions -->
                        <div v-if="isInstructor" class="flex gap-2 shrink-0">
                            <button @click="openEditModal(ann)"
                                    class="text-xs bg-gray-100 text-gray-600 hover:bg-gray-200 px-2.5 py-1 rounded-lg">
                                Edit
                            </button>
                            <button @click="confirmDelete(ann)"
                                    class="text-xs bg-red-50 text-red-500 hover:bg-red-100 px-2.5 py-1 rounded-lg">
                                Delete
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Create / Edit Modal -->
        <div v-if="showModal"
             class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-lg p-6">
                <h2 class="text-lg font-bold text-gray-800 mb-4">
                    {{ editingAnn ? 'Edit Announcement' : 'New Announcement' }}
                </h2>

                <div v-if="modalError" class="bg-red-50 text-red-600 text-sm rounded-lg px-4 py-2 mb-4">
                    {{ modalError }}
                </div>

                <div class="space-y-4">
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Title</label>
                        <input v-model="form.title" type="text" placeholder="Announcement title"
                               class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500" />
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Content</label>
                        <textarea v-model="form.content" rows="4" placeholder="Write your announcement..."
                                  class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500 resize-none" />
                    </div>
                </div>

                <div class="flex justify-end gap-3 mt-6">
                    <button @click="showModal = false"
                            class="px-4 py-2 text-sm text-gray-600 font-medium hover:text-gray-800">
                        Cancel
                    </button>
                    <button @click="submitAnnouncement" :disabled="saving"
                            class="px-4 py-2 bg-primary-600 hover:bg-primary-700 text-white text-sm font-medium rounded-lg disabled:opacity-60 transition-colors">
                        {{ saving ? 'Saving...' : (editingAnn ? 'Save Changes' : 'Post') }}
                    </button>
                </div>
            </div>
        </div>

        <!-- Delete Confirm -->
        <div v-if="showDeleteModal"
             class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-sm p-6 text-center">
                <div class="text-4xl mb-3">🗑️</div>
                <h2 class="text-lg font-bold text-gray-800 mb-2">Delete Announcement?</h2>
                <p class="text-sm text-gray-500 mb-6">This cannot be undone.</p>
                <div class="flex gap-3 justify-center">
                    <button @click="showDeleteModal = false"
                            class="px-4 py-2 text-sm border border-gray-300 rounded-lg hover:bg-gray-50">
                        Cancel
                    </button>
                    <button @click="executeDelete" :disabled="saving"
                            class="px-4 py-2 bg-red-500 text-white text-sm rounded-lg disabled:opacity-60 hover:bg-red-600">
                        {{ saving ? 'Deleting...' : 'Delete' }}
                    </button>
                </div>
            </div>
        </div>

    </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../../stores/authStore'
import { announcementService } from '../../services/announcementService'
import { courseService } from '../../services/courseService'

const router   = useRouter()
const route    = useRoute()
const authStore = useAuthStore()
const user     = authStore.user
const courseId = Number(route.params.courseId)

const announcements = ref([])
const loading       = ref(true)
const courseName    = ref('')
const saving        = ref(false)

const showModal   = ref(false)
const editingAnn  = ref(null)
const form        = ref({ title: '', content: '' })
const modalError  = ref('')

const showDeleteModal = ref(false)
const deletingAnn     = ref(null)

const isInstructor = computed(() => user?.role === 'Instructor')

onMounted(async () => {
  await Promise.all([loadAnnouncements(), loadCourseName()])
})

async function loadAnnouncements() {
  loading.value = true
  try {
    const res = await announcementService.getAll(courseId)
    announcements.value = res.data
  } finally {
    loading.value = false
  }
}

async function loadCourseName() {
  try {
    const res = await courseService.getCourseDetail(courseId)
    courseName.value = res.data.title
  } catch { /* ignore */ }
}

function openCreateModal() {
  editingAnn.value = null
  form.value = { title: '', content: '' }
  modalError.value = ''
  showModal.value = true
}

function openEditModal(ann) {
  editingAnn.value = ann
  form.value = { title: ann.title, content: ann.content }
  modalError.value = ''
  showModal.value = true
}

async function submitAnnouncement() {
  if (!form.value.title.trim() || !form.value.content.trim()) {
    modalError.value = 'Title and content are required.'
    return
  }
  saving.value = true
  modalError.value = ''
  try {
    if (editingAnn.value) {
      await announcementService.update(courseId, editingAnn.value.id, form.value)
    } else {
      await announcementService.create(courseId, form.value)
    }
    showModal.value = false
    await loadAnnouncements()
  } catch (err) {
    modalError.value = err.response?.data?.message || 'Something went wrong.'
  } finally {
    saving.value = false
  }
}

function confirmDelete(ann) {
  deletingAnn.value    = ann
  showDeleteModal.value = true
}

async function executeDelete() {
  saving.value = true
  try {
    await announcementService.remove(courseId, deletingAnn.value.id)
    showDeleteModal.value = false
    await loadAnnouncements()
  } finally {
    saving.value = false
  }
}

function goBack() {
  if (user?.role === 'Instructor') router.push('/instructor/dashboard')
  else router.push(`/student/courses/${courseId}`)
}

function formatDate(dateStr) {
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric', month: 'short', day: 'numeric'
  })
}
</script>