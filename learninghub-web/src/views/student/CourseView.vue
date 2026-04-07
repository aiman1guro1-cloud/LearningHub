<template>
    <div class="min-h-screen bg-gray-50">

        <!-- Navbar -->
        <nav class="bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
            <div class="flex items-center gap-3">
                <button @click="router.push('/student/dashboard')"
                        class="text-gray-400 hover:text-gray-600 transition-colors">
                    ← Back
                </button>
                <span class="text-gray-300">|</span>
                <span class="font-semibold text-gray-800 truncate max-w-xs">{{ course?.title }}</span>
            </div>
            <div class="flex items-center gap-3">
                <!-- Overall progress pill -->
                <div class="flex items-center gap-2 bg-gray-100 rounded-full px-3 py-1">
                    <div class="w-16 bg-gray-300 rounded-full h-1.5">
                        <div class="bg-primary-500 h-1.5 rounded-full transition-all"
                             :style="{ width: progressPercent + '%' }" />
                    </div>
                    <span class="text-xs text-gray-600 font-medium">{{ progressPercent }}%</span>
                </div>

                <!-- NEW: Announcements & Discussions buttons -->
                <button @click="router.push(`/courses/${courseId}/announcements`)"
                        class="text-sm text-gray-500 hover:text-gray-700 font-medium transition-colors">
                    📢 Announcements
                </button>
                <button @click="router.push(`/courses/${courseId}/discussions`)"
                        class="text-sm text-gray-500 hover:text-gray-700 font-medium transition-colors">
                    💬 Discussions
                </button>

                <span class="text-sm text-gray-500">{{ user?.fullName }}</span>
            </div>
        </nav>

        <!-- rest of the template unchanged -->
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
                        <div class="mt-2 bg-gray-200 rounded-full h-1.5">
                            <div class="bg-primary-500 h-1.5 rounded-full transition-all"
                                 :style="{ width: progressPercent + '%' }" />
                        </div>
                    </div>

                    <!-- Completion badge -->
                    <div v-if="progressPercent === 100"
                         class="mx-4 mt-3 bg-green-50 border border-green-200 rounded-lg px-3 py-2 text-center">
                        <p class="text-xs text-green-700 font-semibold">🎉 Course Complete!</p>
                    </div>

                    <div v-for="(mod, idx) in course.modules" :key="mod.id">
                        <!-- Module Header -->
                        <div class="px-4 py-2 bg-gray-50 border-b border-gray-100 border-t">
                            <p class="text-xs font-semibold text-gray-500 uppercase tracking-wide">
                                {{ idx + 1 }}. {{ mod.title }}
                            </p>
                        </div>

                        <!-- Materials -->
                        <div v-if="mod.materials.length === 0"
                             class="px-4 py-2 text-xs text-gray-400 italic">
                            No materials yet
                        </div>

                        <div v-for="mat in mod.materials" :key="mat.id"
                             @click="selectMaterial(mat)"
                             :class="[
                'px-4 py-2.5 border-b border-gray-50 last:border-0 cursor-pointer',
                'flex items-center gap-2 hover:bg-gray-50 transition-colors',
                activeMaterial?.id === mat.id
                  ? 'bg-primary-50 border-l-4 border-l-primary-500'
                  : 'border-l-4 border-l-transparent'
              ]">
                            <span :class="isCompleted(mat.id)
                ? 'text-green-500'
                : 'text-gray-300'" class="text-sm shrink-0">
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

                <!-- Nothing selected -->
                <div v-if="!activeMaterial"
                     class="bg-white rounded-xl border border-gray-200 p-12 text-center">
                    <div class="text-5xl mb-4">👈</div>
                    <p class="text-gray-500">Select a material from the sidebar to begin.</p>
                </div>

                <!-- Material Viewer -->
                <div v-else class="space-y-4">
                    <div class="bg-white rounded-xl border border-gray-200 p-6">

                        <!-- Material Header -->
                        <div class="flex items-start justify-between mb-5">
                            <div>
                                <span :class="{
                  'bg-blue-100 text-blue-700':     activeMaterial.type === 'Video',
                  'bg-green-100 text-green-700':   activeMaterial.type === 'Document',
                  'bg-purple-100 text-purple-700': activeMaterial.type === 'Link'
                }" class="text-xs px-2 py-0.5 rounded font-medium mr-2">
                                    {{ activeMaterial.type }}
                                </span>
                                <h2 class="inline text-lg font-bold text-gray-800">
                                    {{ activeMaterial.title }}
                                </h2>
                            </div>

                            <!-- Complete / Uncomplete button -->
                            <div class="shrink-0 ml-4">
                                <button v-if="!isCompleted(activeMaterial.id)"
                                        @click="markComplete(activeMaterial)"
                                        :disabled="marking"
                                        class="text-sm bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded-lg font-medium disabled:opacity-60 transition-colors">
                                    {{ marking ? 'Saving...' : '✓ Mark Complete' }}
                                </button>
                                <button v-else
                                        @click="unmarkComplete(activeMaterial)"
                                        :disabled="marking"
                                        class="text-sm bg-gray-100 hover:bg-gray-200 text-gray-600 px-4 py-2 rounded-lg font-medium disabled:opacity-60 transition-colors">
                                    {{ marking ? 'Saving...' : '↩ Mark Incomplete' }}
                                </button>
                            </div>
                        </div>

                        <!-- Video Player -->
                        <div v-if="activeMaterial.type === 'Video' && activeMaterial.fileUrl"
                             class="rounded-xl overflow-hidden bg-black aspect-video mb-2">
                            <video :src="apiBase + activeMaterial.fileUrl"
                                   controls class="w-full h-full" />
                        </div>

                        <!-- Document -->
                        <div v-else-if="activeMaterial.type === 'Document' && activeMaterial.fileUrl"
                             class="border border-gray-200 rounded-xl p-5 flex items-center gap-4">
                            <div class="w-12 h-12 bg-green-100 rounded-xl flex items-center justify-center text-2xl shrink-0">
                                📄
                            </div>
                            <div>
                                <p class="text-sm font-semibold text-gray-700 mb-0.5">{{ activeMaterial.title }}</p>
                                <a :href="apiBase + activeMaterial.fileUrl" target="_blank"
                                   class="text-sm text-primary-600 hover:underline font-medium">
                                    Open Document ↗
                                </a>
                            </div>
                        </div>

                        <!-- Link -->
                        <div v-else-if="activeMaterial.type === 'Link' && activeMaterial.fileUrl"
                             class="border border-gray-200 rounded-xl p-5 flex items-center gap-4">
                            <div class="w-12 h-12 bg-purple-100 rounded-xl flex items-center justify-center text-2xl shrink-0">
                                🔗
                            </div>
                            <div>
                                <p class="text-sm font-semibold text-gray-700 mb-0.5">{{ activeMaterial.title }}</p>
                                <a :href="activeMaterial.fileUrl" target="_blank"
                                   class="text-sm text-primary-600 hover:underline font-medium">
                                    Open Link ↗
                                </a>
                            </div>
                        </div>

                        <!-- No file uploaded -->
                        <div v-else
                             class="border-2 border-dashed border-gray-200 rounded-xl p-10 text-center text-gray-400">
                            <p class="text-3xl mb-2">📭</p>
                            <p class="text-sm">No file uploaded for this material yet.</p>
                        </div>
                    </div>

                    <!-- Next material button -->
                    <div v-if="nextMaterial" class="flex justify-end">
                        <button @click="selectMaterial(nextMaterial)"
                                class="bg-white border border-gray-200 hover:border-primary-300 text-gray-700 hover:text-primary-700 px-4 py-2 rounded-lg text-sm font-medium transition-colors">
                            Next: {{ nextMaterial.title }} →
                        </button>
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

    const router = useRouter()
    const route = useRoute()
    const authStore = useAuthStore()
    const user = authStore.user
    const courseId = Number(route.params.courseId)
    const apiBase = import.meta.env.VITE_API_URL.replace('/api', '')

    const course = ref(null)
    const loading = ref(true)
    const activeMaterial = ref(null)
    const completedIds = ref([])
    const marking = ref(false)

    // Flat list of all materials in order
    const allMaterials = computed(() =>
        course.value?.modules?.flatMap(m => m.materials) ?? []
    )

    const nextMaterial = computed(() => {
        if (!activeMaterial.value) return null
        const idx = allMaterials.value.findIndex(m => m.id === activeMaterial.value.id)
        return idx >= 0 && idx < allMaterials.value.length - 1
            ? allMaterials.value[idx + 1]
            : null
    })

    const totalCount = computed(() => allMaterials.value.length)

    const completedCount = computed(() => completedIds.value.length)

    const progressPercent = computed(() =>
        totalCount.value > 0
            ? Math.round((completedCount.value / totalCount.value) * 100)
            : 0
    )

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
            if (allMaterials.value.length > 0) {
                activeMaterial.value = allMaterials.value[0]
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
        } catch { /* no progress yet */ }
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
            if (!completedIds.value.includes(mat.id)) {
                completedIds.value.push(mat.id)
            }
        } catch (err) {
            alert(err.response?.data?.message || 'Could not mark complete.')
        } finally {
            marking.value = false
        }
    }

    async function unmarkComplete(mat) {
        marking.value = true
        try {
            await progressService.unmarkComplete(courseId, mat.id)
            completedIds.value = completedIds.value.filter(id => id !== mat.id)
        } catch (err) {
            alert(err.response?.data?.message || 'Could not unmark.')
        } finally {
            marking.value = false
        }
    }
</script>