<template>
    <div class="animate-pulse space-y-3">
        <div v-for="n in lines" :key="n"
             class="bg-gray-200 rounded-lg"
             :style="{ height: height + 'px', width: n === lines ? '60%' : '100%' }" />
    </div>
</template>

<script setup>
defineProps({
  lines:  { type: Number, default: 3 },
  height: { type: Number, default: 16 }
})
</script>