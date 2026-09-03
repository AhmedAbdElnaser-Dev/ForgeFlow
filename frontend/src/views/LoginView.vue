<script setup>
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import AutodeskMark from '@/components/AutodeskMark.vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()
const route = useRoute()

const email = ref('')
const password = ref('')
const showPassword = ref(false)
const isSubmitting = ref(false)
const form = ref(null)

const notice = ref('')
const isNoticeOpen = ref(false)

const emailRules = [
  (value) => !!value || 'Email is required',
  (value) => /.+@.+\..+/.test(value) || 'Enter a valid email',
]
const passwordRules = [(value) => !!value || 'Password is required']

async function onSubmit() {
  const { valid } = await form.value.validate()
  if (!valid) {
    return
  }

  isSubmitting.value = true

  try {
    await auth.signIn({ email: email.value, password: password.value })
    await router.replace(route.query.redirect || { name: 'profile' })
  } catch (error) {
    notice.value = error.response?.data?.detail ?? 'Could not sign in. Please try again.'
    isNoticeOpen.value = true
  } finally {
    isSubmitting.value = false
  }
}

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

      <v-form ref="form" class="text-start" @submit.prevent="onSubmit">
        <v-text-field
          v-model="email"
          label="Email"
          type="email"
          autocomplete="username"
          variant="outlined"
          density="comfortable"
          prepend-inner-icon="mdi-email-outline"
          :rules="emailRules"
          :disabled="isSubmitting"
        />

        <v-text-field
          v-model="password"
          label="Password"
          :type="showPassword ? 'text' : 'password'"
          autocomplete="current-password"
          variant="outlined"
          density="comfortable"
          prepend-inner-icon="mdi-lock-outline"
          :append-inner-icon="showPassword ? 'mdi-eye-off-outline' : 'mdi-eye-outline'"
          :rules="passwordRules"
          :disabled="isSubmitting"
          @click:append-inner="showPassword = !showPassword"
        />

        <v-btn
          type="submit"
          class="login__button"
          color="primary"
          block
          height="48"
          :loading="isSubmitting"
        >
          Sign in
        </v-btn>
      </v-form>

      <div class="login__divider">
        <span>or</span>
      </div>

      <v-btn
        class="login__autodesk"
        variant="outlined"
        block
        height="48"
        :disabled="isSubmitting"
        @click="onLoginWithAutodesk"
      >
        <template #prepend>
          <AutodeskMark :size="18" />
        </template>
        Login with Autodesk
      </v-btn>
    </div>

    <v-snackbar v-model="isNoticeOpen" color="error" :timeout="5000" :text="notice" />
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
  padding: 44px 36px 36px;
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
  margin-bottom: 28px;
  font-size: 2rem;
  font-weight: 600;
  letter-spacing: -0.03em;
  line-height: 1.1;
}

.login__button {
  margin-top: 4px;
  font-weight: 600;
  letter-spacing: 0;
  box-shadow: 0 12px 28px -14px rgba(34, 197, 94, 0.9);
}

.login__divider {
  display: flex;
  align-items: center;
  gap: 12px;
  margin: 20px 0;
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: rgba(245, 245, 245, 0.45);
}

.login__divider::before,
.login__divider::after {
  content: '';
  flex: 1;
  height: 1px;
  background: rgba(148, 163, 184, 0.18);
}

.login__autodesk {
  font-weight: 600;
  letter-spacing: 0;
}
</style>
