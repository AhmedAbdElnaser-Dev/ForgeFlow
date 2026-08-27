<script setup>
// Placeholder Autodesk "A" mark. Swap for the official asset from the Autodesk
// brand kit before anything ships publicly.
defineProps({
  size: { type: [Number, String], default: 20 },
})
</script>

<template>
  <svg
    :width="size"
    :height="size"
    viewBox="0 0 24 24"
    fill="none"
    aria-hidden="true"
    focusable="false"
  >
    <path
      d="M12 2.75 2.5 21.25h4.62L12 11.6l4.88 9.65h4.62L12 2.75Z"
      fill="currentColor"
    />
    <path d="M9.05 17.2h5.9l1.5 2.95h-8.9l1.5-2.95Z" fill="currentColor" opacity=".55" />
  </svg>
</template>
