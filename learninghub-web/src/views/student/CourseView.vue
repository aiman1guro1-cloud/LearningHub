<template>
    <div class="min-h-screen bg-gray-50">

        <!-- Navbar -->
        <nav class="bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
            <div class="flex items-center gap-3">
                <button @click="router.push('/student/dashboard')"
                        class="text-gray-400 hover:text-gray-600">
                    ← Back
                </button>
                <span class="text-gray-300">|</span>
                <span class="font-semibold text-gray-800 truncate max-w-xs">{{ course?.title }}</span>
            </div>
            <span class="text-sm text-gray-500">{{ user?.fullName }}</span>
        </nav>

        <div v-if="loading" class="text-center py-16 text-gray-400">Loading course...</div>

        <div v-else-if="!course" class="text-center py-16">
            <p class="text-gray-500">Course not found or you are not enrolled.</p>
            <button @click="router.push('/student/dashboard')"
                    class="mt-4 text-primary-600 hover:underline text-sm">
                Back to Dashboard
            </button>
        </div>

        <div v-else class="max-w-5xl mx-auto px-6 py-8 flex gap-6">

            <!-- Sidebar: Module List -->
            <div class="w-72 shrink-0">
                <div class="bg-white rounded-xl border border-gray-200 overflow-hidden sticky top-6">
                    <div class="px-4 py-3 border-b border-gray-100 bg-gray-50">
                        <h3 class="text-sm font-semibold text-gray-700">Course Content</h3>
                        <p class="text-xs text-gray-400 mt-0.5">
                            {{ completedCount }}/{{ totalCount }} completed
                        </p>
                        <!-- Progress bar -->
                        <div class="mt-2 bg-gray-200 rounded-full h-1.5">
                            <div class="bg-primary-500 h-1.5 rounded-full transition-all"
                                 :style="{ width: progressPercent + '%' }" />
                        </div>
                    </div>

                    <div v-for="(mod, idx) in course.modules" :key="mod.id">
                        <div class="px-4 py-2 bg-gray-50 border-b border-gray-100">
                            <p class="text-xs font-semibold text-gray-500 uppercase tracking-wide">
                                Module {{ idx + 1 }}: {{ mod.title }}
                            </p>
                        </div>
                        <div v-for="mat in mod.materials" :key="mat.id"
                             @click="selectMaterial(mat)"
                             :class="[
                'px-4 py-2.5 border-b border-gray-50 cursor-pointer flex items-center gap-2 hover:bg-gray-50 transition-colors',
                activeMaterial?.id === mat.id ? 'bg-primary-50 border-l-2 border-l-primary-500' : ''
              ]">
                            <span :class="isCompleted(mat.id) ? 'text-green-500' : 'text-gray-300'" class="text-sm">
                                {{ isCompleted(mat.id) ? '✓' : '○' }}
                            </span>
                            <div class="flex-1 min-w-0">
                                <p class="text-sm text-gray-700 truncate">{{ mat.title }}</p>
                                <p class="text-xs text-gray-400">{{ mat.type }}</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Main Content Area -->
            <div class="flex-1 min-w-0">

                <!-- No material selected -->
                <div v-if="!activeMaterial"
                     class="bg-white rounded-xl border border-gray-200 p-12 text-center">
                    <div class="text-5xl mb-4">👈</div>
                    <p class="text-gray-500">Select a material from the sidebar to get started.</p>
                </div>

                <!-- Material Viewer -->
                <div v-else class="bg-white rounded-xl border border-gray-200 p-6">
                    <div class="flex items-start justify-between mb-4">
                        <div>
                            <span :class="{
                'bg-blue-100 text-blue-700':   activeMaterial.type === 'Video',
                'bg-green-100 text-green-700': activeMaterial.type === 'Document',
                'bg-purple-100 text-purple-700': activeMaterial.type === 'Link'
              }" class="text-xs px-2 py-0.5 rounded font-medium mr-2">
                                {{ activeMaterial.type }}
                            </span>
                            <h2 class="inline text-lg font-bold text-gray-800">{{ activeMaterial.title }}</h2>
                        </div>
                        <button v-if="!isCompleted(activeMaterial.id)"
                                @click="markComplete(activeMaterial)"
                                :disabled="marking"
                                class="shrink-0 ml-4 text-sm bg-green-500 hover:bg-green-600 text-white px-3 py-1.5 rounded-lg font-medium disabled:opacity-60">
                            {{ marking ? 'Saving...' : 'Mark Complete' }}
                        </button>
                        <span v-else class="shrink-0 ml-4 text-sm text-green-600 font-medium flex items-center gap-1">
                            ✓ Completed
                        </span>
                    </div>

                    <!-- Video Player -->
                    <div v-if="activeMaterial.type === 'Video' && activeMaterial.fileUrl"
                         class="rounded-lg overflow-hidden bg-black aspect-video mb-4">
                        <video :src="apiBase + activeMaterial.fileUrl"
                               controls class="w-full h-full" />
                    </div>

                    <!-- Document Link -->
                    <div v-else-if="activeMaterial.type === 'Document' && activeMaterial.fileUrl"
                         class="border border-gray-200 rounded-lg p-4 flex items-center gap-3 mb-4">
                        <span class="text-3xl">📄</span>
                        <div>
                            <p class="text-sm font-medium text-gray-700">{{ activeMaterial.title }}</p>
                            <a :href="apiBase + activeMaterial.fileUrl" target="_blank"
                               class="text-sm text-primary-600 hover:underline">
                                Open Document ↗
                            </a>
                        </div>
                    </div>

                    <!-- External Link -->
                    <div v-else-if="activeMaterial.type === 'Link'"
                         class="border border-gray-200 rounded-lg p-4 flex items-center gap-3 mb-4">
                        <span class="text-3xl">🔗</span>
                        <div>
                            <p class="text-sm font-medium text-gray-700">{{ activeMaterial.title }}</p>
                            <a :href="activeMaterial.fileUrl" target="_blank"
                               class="text-sm text-primary-600 hover:underline">
                                Open Link ↗
                            </a>
                        </div>
                    </div>

                    <!-- No file yet -->
                    <div v-else
                         class="border-2 border-dashed border-gray-200 rounded-lg p-8 text-center text-gray-400 mb-4">
                        No file uploaded yet.
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../../stores/authStore'
import { courseService } from '../../services/courseService'
import { progressService } from '../../services/progressService'

const router   = useRouter()
const route    = useRoute()
const authStore = useAuthStore()
const user     = authStore.user
const courseId = Number(route.params.courseId)
const apiBase  = import.meta.env.VITE_API_URL.replace('/api', '')

const course         = ref(null)
const loading        = ref(true)
const activeMaterial = ref(null)
const completedIds   = ref([])
const marking        = ref(false)

onMounted(async () => {
  await loadCourse()
  await loadProgress()
})

async function loadCourse() {
  loading.value = true
  try {
    const res = await courseService.getCourseDetail(courseId)
    course.value = res.data
    // Auto-select first material
    if (res.data.modules?.length > 0 && res.data.modules[0].materials?.length > 0) {
      activeMaterial.value = res.data.modules[0].materials[0]
    }
  } catch {
    course.value = null
  } finally {
    loading.value = false
  }
}

async function loadProgress() {
  try {
    const res = await progressService.getMyProgress(courseId)
    completedIds.value = res.data.map(p => p.materialId)
  } catch { /* not enrolled or no progress yet */ }
}

function selectMaterial(mat) {
  activeMaterial.value = mat
}

function isCompleted(materialId) {
  return completedIds.value.includes(materialId)
}

async function markComplete(mat) {
  marking.value = true
  try {
    await progressService.markComplete(courseId, mat.id)
    completedIds.value.push(mat.id)
  } finally {
    marking.value = false
  }
}

const totalCount = computed(() =>
  course.value?.modules?.reduce((sum, m) => sum + m.materials.length, 0) ?? 0
)

const completedCount = computed(() => completedIds.value.length)

const progressPercent = computed(() =>
  totalCount.value > 0
    ? Math.round((completedCount.value / totalCount.value) * 100)
    : 0
)
</script>