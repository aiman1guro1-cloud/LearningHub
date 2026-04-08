<template>
    <div class="text-center py-16 px-4">
        <div class="text-5xl mb-4">{{ icon }}</div>
        <h3 class="text-base font-semibold text-gray-700 mb-1">{{ title }}</h3>
        <p class="text-sm text-gray-400 mb-5 max-w-xs mx-auto">{{ description }}</p>
        <button v-if="actionLabel" @click="$emit('action')"
                class="bg-primary-600 hover:bg-primary-700 text-white px-5 py-2 rounded-xl
      text-sm font-medium transition-colors">
            {{ actionLabel }}
        </button>
    </div>
</template>

<script setup>
defineProps({
  icon:        { type: String, default: '📭' },
  title:       { type: String, required: true },
  description: { type: String, default: '' },
  actionLabel: { type: String, default: '' }
})
defineEmits(['action'])
</script>