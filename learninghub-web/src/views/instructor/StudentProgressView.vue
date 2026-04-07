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
                <span class="font-semibold text-gray-800">Student Progress</span>
            </div>
            <span class="text-sm text-gray-500">{{ courseName }}</span>
        </nav>

        <div class="max-w-4xl mx-auto px-6 py-8">

            <div class="mb-6">
                <h1 class="text-2xl font-bold text-gray-800">Student Progress</h1>
                <p class="text-gray-500 text-sm mt-1">
                    {{ progressList.length }} student{{ progressList.length !== 1 ? 's' : '' }} enrolled
                </p>
            </div>

            <div v-if="loading" class="text-center py-16 text-gray-400">Loading...</div>

            <div v-else-if="progressList.length === 0"
                 class="text-center py-16">
                <div class="text-5xl mb-4">👥</div>
                <p class="text-gray-500">No students enrolled yet.</p>
            </div>

            <div v-else class="bg-white rounded-xl border border-gray-200 overflow-hidden">
                <!-- Table Header -->
                <div class="grid grid-cols-5 gap-4 px-5 py-3 bg-gray-50 border-b border-gray-200 text-xs font-semibold text-gray-500 uppercase tracking-wide">
                    <div class="col-span-2">Student</div>
                    <div>Enrolled</div>
                    <div>Progress</div>
                    <div>Completion</div>
                </div>

                <!-- Table Rows -->
                <div v-for="item in progressList" :key="item.studentId"
                     class="grid grid-cols-5 gap-4 px-5 py-4 border-b border-gray-100 last:border-0 items-center hover:bg-gray-50 transition-colors">

                    <div class="col-span-2">
                        <p class="text-sm font-semibold text-gray-800">{{ item.studentName }}</p>
                        <p class="text-xs text-gray-400">{{ item.studentEmail }}</p>
                    </div>

                    <div class="text-sm text-gray-500">
                        {{ formatDate(item.enrolledAt) }}
                    </div>

                    <div class="flex items-center gap-2">
                        <div class="flex-1 bg-gray-100 rounded-full h-2">
                            <div :class="item.progressPercent === 100
                  ? 'bg-green-500'
                  : 'bg-primary-500'"
                                 class="h-2 rounded-full transition-all"
                                 :style="{ width: item.progressPercent + '%' }" />
                        </div>
                        <span class="text-xs text-gray-500 whitespace-nowrap w-10 text-right">
                            {{ item.progressPercent }}%
                        </span>
                    </div>

                    <div>
                        <span v-if="item.progressPercent === 100"
                              class="inline-flex items-center gap-1 text-xs bg-green-100 text-green-700 px-2 py-0.5 rounded-full font-medium">
                            ✓ Complete
                        </span>
                        <span v-else
                              class="text-xs text-gray-500">
                            {{ item.completedMaterials }}/{{ item.totalMaterials }} materials
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { progressService } from '../../services/progressService'
import { courseService } from '../../services/courseService'

const router   = useRouter()
const route    = useRoute()
const courseId = Number(route.params.courseId)

const progressList = ref([])
const courseName   = ref('')
const loading      = ref(true)

onMounted(async () => {
  loading.value = true
  try {
    const [progressRes, courseRes] = await Promise.all([
      progressService.getStudentProgress(courseId),
      courseService.getCourseDetail(courseId)
    ])
    progressList.value = progressRes.data
    courseName.value   = courseRes.data.title
  } finally {
    loading.value = false
  }
})

function formatDate(dateStr) {
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric', month: 'short', day: 'numeric'
  })
}
</script>