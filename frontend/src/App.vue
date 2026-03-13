<script setup lang="ts">
import DashboardView from '@/views/DashboardView.vue'
import { useKeepAlive } from '@/composables/useKeepAlive'

// Render 슬립 방지: 백엔드 헬스체크를 10분마다 자동 호출
useKeepAlive()
</script>

<template>
  <DashboardView />
</template>
