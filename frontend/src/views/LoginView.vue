<script setup>
import AutodeskMark from '@/components/AutodeskMark.vue'

function onLoginWithAutodesk() {
  console.log('Autodesk login triggered')
}
</script>

<template>
  <div class="login">
    <div class="login__grid" aria-hidden="true" />
    <div class="login__vignette" aria-hidden="true" />
    <div class="login__glow" aria-hidden="true" />

    <div class="login__card">
      <span class="login__seam" aria-hidden="true" />

      <h1 class="login__wordmark">Forge<span class="text-primary">Flow</span></h1>

      <v-btn
        class="login__button"
        color="primary"
        block
        height="48"
        @click="onLoginWithAutodesk"
      >
        <template #prepend>
          <AutodeskMark :size="18" />
        </template>
        Login with Autodesk
      </v-btn>
    </div>
  </div>
</template>

<style scoped>
.login {
  position: relative;
  display: grid;
  place-items: center;
  min-height: 100dvh;
  padding: 24px;
  overflow: hidden;
  background: #07080b;
}

.login__grid {
  position: absolute;
  inset: 0;
  background-image:
    linear-gradient(rgba(148, 163, 184, 0.22) 1px, transparent 1px),
    linear-gradient(90deg, rgba(148, 163, 184, 0.22) 1px, transparent 1px);
  background-size: 40px 40px;
  -webkit-mask-image: radial-gradient(ellipse 85% 75% at 50% 50%, #000 25%, transparent 100%);
  mask-image: radial-gradient(ellipse 85% 75% at 50% 50%, #000 25%, transparent 100%);
}

/* Deepens the corners so the grid never reaches a hard edge. */
.login__vignette {
  position: absolute;
  inset: 0;
  background: radial-gradient(ellipse 70% 60% at 50% 50%, transparent 30%, #07080b 100%);
}

.login__glow {
  position: absolute;
  width: min(560px, 88vw);
  aspect-ratio: 1;
  border-radius: 50%;
  background: radial-gradient(
    circle,
    rgba(34, 197, 94, 0.2) 0%,
    rgba(34, 197, 94, 0.05) 38%,
    transparent 68%
  );
  pointer-events: none;
}

.login__card {
  position: relative;
  width: 100%;
  max-width: 384px;
  padding: 52px 40px 40px;
  text-align: center;
  border-radius: 16px;
  border: 1px solid rgba(148, 163, 184, 0.14);
  background: linear-gradient(180deg, #141b26 0%, #0d121b 100%);
  box-shadow:
    0 1px 0 rgba(255, 255, 255, 0.05) inset,
    0 40px 80px -32px rgba(0, 0, 0, 1),
    0 0 70px -30px rgba(34, 197, 94, 0.45);
}

/* Hairline of green light across the top edge of the card. */
.login__seam {
  position: absolute;
  top: -1px;
  left: 15%;
  width: 70%;
  height: 1px;
  background: linear-gradient(90deg, transparent, rgba(34, 197, 94, 0.75), transparent);
}

.login__wordmark {
  margin-bottom: 36px;
  font-size: 2rem;
  font-weight: 600;
  letter-spacing: -0.03em;
  line-height: 1.1;
}

.login__button {
  font-weight: 600;
  letter-spacing: 0;
  box-shadow: 0 12px 28px -14px rgba(34, 197, 94, 0.9);
  transition:
    transform 0.15s ease,
    box-shadow 0.15s ease;
}

.login__button:hover {
  transform: translateY(-1px);
  box-shadow: 0 16px 34px -14px rgba(34, 197, 94, 1);
}

.login__button:active {
  transform: translateY(0);
}

@media (prefers-reduced-motion: reduce) {
  .login__button {
    transition: none;
  }

  .login__button:hover {
    transform: none;
  }
}
</style>
