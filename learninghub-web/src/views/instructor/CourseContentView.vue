<template>
    <div class="min-h-screen bg-gray-50">

        <!-- Navbar -->
        <nav class="bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
            <div class="flex items-center gap-3">
                <button @click="router.push('/instructor/dashboard')"
                        class="text-gray-400 hover:text-gray-600 transition-colors">
                    ← Back
                </button>
                <span class="text-gray-300">|</span>
                <span class="font-semibold text-gray-800 truncate max-w-xs">{{ course?.title }}</span>
            </div>
            <span class="text-sm text-gray-500">Content Manager</span>
        </nav>

        <div class="max-w-4xl mx-auto px-6 py-8">

            <div v-if="loading" class="text-center py-16 text-gray-400">Loading...</div>

            <div v-else>

                <!-- Modules -->
                <div class="flex items-center justify-between mb-4">
                    <h2 class="text-lg font-bold text-gray-800">Modules</h2>
                    <button @click="openModuleModal(null)"
                            class="bg-primary-600 hover:bg-primary-700 text-white px-3 py-1.5 rounded-lg text-sm font-medium">
                        + Add Module
                    </button>
                </div>

                <div v-if="modules.length === 0" class="text-center py-10 text-gray-400">
                    No modules yet. Add your first module.
                </div>

                <div v-for="(mod, idx) in modules" :key="mod.id" class="mb-4">
                    <!-- Module Card -->
                    <div class="bg-white rounded-xl border border-gray-200">
                        <div class="flex items-center justify-between px-5 py-4 border-b border-gray-100">
                            <div class="flex items-center gap-3">
                                <span class="w-7 h-7 bg-primary-100 text-primary-700 rounded-full text-xs font-bold flex items-center justify-center">
                                    {{ idx + 1 }}
                                </span>
                                <span class="font-semibold text-gray-800">{{ mod.title }}</span>
                                <span class="text-xs text-gray-400">{{ mod.materials.length }} materials</span>
                            </div>
                            <div class="flex gap-2">
                                <button @click="openModuleModal(mod)"
                                        class="text-xs bg-gray-100 text-gray-600 hover:bg-gray-200 px-2.5 py-1 rounded-lg">
                                    Edit
                                </button>
                                <button @click="confirmDeleteModule(mod)"
                                        class="text-xs bg-red-50 text-red-500 hover:bg-red-100 px-2.5 py-1 rounded-lg">
                                    Delete
                                </button>
                            </div>
                        </div>

                        <!-- Materials List -->
                        <div class="px-5 py-3 space-y-2">
                            <div v-for="mat in mod.materials" :key="mat.id"
                                 class="flex items-center justify-between py-2 border-b border-gray-50 last:border-0">
                                <div class="flex items-center gap-3">
                                    <span :class="{
                    'bg-blue-100 text-blue-700':   mat.type === 'Video',
                    'bg-green-100 text-green-700': mat.type === 'Document',
                    'bg-purple-100 text-purple-700': mat.type === 'Link'
                  }" class="text-xs px-2 py-0.5 rounded font-medium">{{ mat.type }}</span>
                                    <span class="text-sm text-gray-700">{{ mat.title }}</span>
                                </div>
                                <button @click="confirmDeleteMaterial(mod.id, mat)"
                                        class="text-xs text-red-400 hover:text-red-600">
                                    Remove
                                </button>
                            </div>

                            <!-- Add Material -->
                            <button @click="openMaterialModal(mod.id)"
                                    class="w-full text-left text-sm text-primary-600 hover:text-primary-700 py-1.5 font-medium">
                                + Add Material
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Module Modal -->
        <div v-if="showModuleModal"
             class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-md p-6">
                <h2 class="text-lg font-bold text-gray-800 mb-4">
                    {{ editingModule ? 'Edit Module' : 'New Module' }}
                </h2>
                <div class="space-y-4">
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Module Title</label>
                        <input v-model="moduleForm.title" type="text" placeholder="e.g. Introduction"
                               class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500" />
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Order</label>
                        <input v-model.number="moduleForm.orderIndex" type="number" min="0"
                               class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500" />
                    </div>
                </div>
                <div class="flex justify-end gap-3 mt-6">
                    <button @click="showModuleModal = false" class="px-4 py-2 text-sm text-gray-600 font-medium">Cancel</button>
                    <button @click="submitModule" :disabled="saving"
                            class="px-4 py-2 bg-primary-600 text-white text-sm font-medium rounded-lg disabled:opacity-60">
                        {{ saving ? 'Saving...' : 'Save' }}
                    </button>
                </div>
            </div>
        </div>

        <!-- Material Modal -->
        <div v-if="showMaterialModal"
             class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-md p-6">
                <h2 class="text-lg font-bold text-gray-800 mb-4">Add Material</h2>
                <div class="space-y-4">
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Title</label>
                        <input v-model="materialForm.title" type="text" placeholder="e.g. Lecture 1 Video"
                               class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500" />
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Type</label>
                        <select v-model="materialForm.type"
                                class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500">
                            <option value="Video">Video</option>
                            <option value="Document">Document</option>
                            <option value="Link">Link</option>
                        </select>
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">File (optional)</label>
                        <input type="file" @change="onFileChange"
                               class="w-full text-sm text-gray-500 file:mr-3 file:py-1.5 file:px-3 file:rounded-lg file:border-0 file:bg-primary-50 file:text-primary-700 hover:file:bg-primary-100" />
                    </div>
                </div>
                <div class="flex justify-end gap-3 mt-6">
                    <button @click="showMaterialModal = false" class="px-4 py-2 text-sm text-gray-600 font-medium">Cancel</button>
                    <button @click="submitMaterial" :disabled="saving"
                            class="px-4 py-2 bg-primary-600 text-white text-sm font-medium rounded-lg disabled:opacity-60">
                        {{ saving ? 'Uploading...' : 'Add Material' }}
                    </button>
                </div>
            </div>
        </div>

        <!-- Delete Confirm -->
        <div v-if="showDeleteModal"
             class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-sm p-6 text-center">
                <div class="text-4xl mb-3">🗑️</div>
                <h2 class="text-lg font-bold text-gray-800 mb-2">Delete "{{ deletingItem?.title }}"?</h2>
                <p class="text-sm text-gray-500 mb-6">This action cannot be undone.</p>
                <div class="flex gap-3 justify-center">
                    <button @click="showDeleteModal = false"
                            class="px-4 py-2 text-sm border border-gray-300 rounded-lg hover:bg-gray-50">
                        Cancel
                    </button>
                    <button @click="executeDelete" :disabled="saving"
                            class="px-4 py-2 bg-red-500 text-white text-sm rounded-lg disabled:opacity-60">
                        {{ saving ? 'Deleting...' : 'Delete' }}
                    </button>
                </div>
            </div>
        </div>

    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { courseService } from '../../services/courseService'

const router = useRouter()
const route  = useRoute()
const courseId = Number(route.params.courseId)

const course  = ref(null)
const modules = ref([])
const loading = ref(true)
const saving  = ref(false)

// Module modal
const showModuleModal = ref(false)
const editingModule   = ref(null)
const moduleForm      = ref({ title: '', orderIndex: 0 })

// Material modal
const showMaterialModal = ref(false)
const activeMaterialModuleId = ref(null)
const materialForm = ref({ title: '', type: 'Video', orderIndex: 0 })
const selectedFile = ref(null)

// Delete modal
const showDeleteModal = ref(false)
const deletingItem    = ref(null)
const deleteType      = ref('') // 'module' or 'material'
const deleteMeta      = ref(null)

onMounted(loadContent)

async function loadContent() {
  loading.value = true
  try {
    const res = await courseService.getCourseDetail(courseId)
    course.value  = res.data
    modules.value = res.data.modules || []
  } finally {
    loading.value = false
  }
}

// ── Modules ─────────────────────────────────────────────
function openModuleModal(mod) {
  editingModule.value = mod
  moduleForm.value = mod
    ? { title: mod.title, orderIndex: mod.orderIndex }
    : { title: '', orderIndex: modules.value.length }
  showModuleModal.value = true
}

async function submitModule() {
  if (!moduleForm.value.title.trim()) return
  saving.value = true
  try {
    if (editingModule.value) {
      await courseService.updateModule(courseId, editingModule.value.id, moduleForm.value)
    } else {
      await courseService.createModule(courseId, moduleForm.value)
    }
    showModuleModal.value = false
    await loadContent()
  } finally {
    saving.value = false
  }
}

// ── Materials ────────────────────────────────────────────
function openMaterialModal(moduleId) {
  activeMaterialModuleId.value = moduleId
  materialForm.value = { title: '', type: 'Video', orderIndex: 0 }
  selectedFile.value = null
  showMaterialModal.value = true
}

function onFileChange(e) {
  selectedFile.value = e.target.files[0] || null
}

async function submitMaterial() {
  if (!materialForm.value.title.trim()) return
  saving.value = true
  try {
    const formData = new FormData()
    formData.append('title',      materialForm.value.title)
    formData.append('type',       materialForm.value.type)
    formData.append('orderIndex', materialForm.value.orderIndex)
    if (selectedFile.value) formData.append('file', selectedFile.value)

    await courseService.createMaterial(activeMaterialModuleId.value, formData)
    showMaterialModal.value = false
    await loadContent()
  } finally {
    saving.value = false
  }
}

// ── Delete ───────────────────────────────────────────────
function confirmDeleteModule(mod) {
  deletingItem.value  = mod
  deleteType.value    = 'module'
  deleteMeta.value    = null
  showDeleteModal.value = true
}

function confirmDeleteMaterial(moduleId, mat) {
  deletingItem.value  = mat
  deleteType.value    = 'material'
  deleteMeta.value    = { moduleId }
  showDeleteModal.value = true
}

async function executeDelete() {
  saving.value = true
  try {
    if (deleteType.value === 'module') {
      await courseService.deleteModule(courseId, deletingItem.value.id)
    } else {
      await courseService.deleteMaterial(deleteMeta.value.moduleId, deletingItem.value.id)
    }
    showDeleteModal.value = false
    await loadContent()
  } finally {
    saving.value = false
  }
}
</script>