# Yapılması Gerekenler - CI/CD Kurulumu

## 🎯 Genel Bakış

Bu dokümanda, yeni oluşturulan production-grade CI/CD pipeline'larını aktif hale getirmek için yapılması gereken tüm adımlar detaylı olarak açıklanmıştır.

---

## ✅ Öncelikli Görevler (Kritik)

### 1. GitHub Secrets Yapılandırması (Zorunlu)

#### 1.1. NuGet API Key Oluşturma

**Neden Gerekli:** NuGet paketlerini yayınlamak için gerekli

**Adımlar:**
1. https://www.nuget.org/ adresine git
2. Hesabına giriş yap (yoksa kayıt ol)
3. Sağ üst köşeden **API Keys** sekmesine tıkla
4. **Create** butonuna bas
5. Ayarları yapılandır:
   - **Key Name:** `GitHub Actions - Advanced Concepts`
   - **Expiration:** `365 days` (1 yıl)
   - **Scopes:** `Push new packages and package versions`
   - **Select Packages:** `All Packages` veya sadece `AdvancedConcepts.*`
6. **Create** butonuna bas
7. API Key'i kopyala (bir daha göremezsin!)

#### 1.2. SonarCloud Token Oluşturma

**Neden Gerekli:** Kod kalitesi analizi için gerekli

**Adımlar:**
1. https://sonarcloud.io/ adresine git
2. GitHub hesabınla giriş yap
3. Sağ üst köşeden **My Account** → **Security** sekmesine git
4. **Generate Tokens** bölümüne gel
5. Token oluştur:
   - **Name:** `GitHub Actions CI`
   - **Type:** `User Token`
   - **Expiration:** `No expiration` veya `1 year`
6. **Generate** butonuna bas
7. Token'ı kopyala

#### 1.3. GitHub Secrets Ekleme

**Adımlar:**
1. GitHub repository'ne git
2. **Settings** → **Secrets and variables** → **Actions** sekmesine tıkla
3. **New repository secret** butonuna bas
4. İlk secret'ı ekle:
   - **Name:** `NUGET_API_KEY`
   - **Secret:** (NuGet API key'ini yapıştır)
   - **Add secret** butonuna bas
5. İkinci secret'ı ekle:
   - **Name:** `SONAR_TOKEN`
   - **Secret:** (SonarCloud token'ını yapıştır)
   - **Add secret** butonuna bas

**Doğrulama:**
- Settings → Secrets and variables → Actions sayfasında 2 secret görmelisin:
  - ✅ `NUGET_API_KEY`
  - ✅ `SONAR_TOKEN`

---

### 2. SonarCloud Projesini Yapılandırma

#### 2.1. Projeyi SonarCloud'a İmport Etme

**Adımlar:**
1. https://sonarcloud.io/ adresine git
2. Sağ üst köşeden **+** → **Analyze new project** seçeneğine tıkla
3. **GitHub** seçeneğini seç
4. Repository listesinden projeyi seç:
   - `CSharp-Covariance-Polymorphism-Exercises`
5. **Set Up** butonuna bas
6. Analysis Method olarak **GitHub Actions** seç
7. Bilgileri not et:
   - **Organization:** `dogaaydinn`
   - **Project Key:** `dogaaydinn_CSharp-Covariance-Polymorphism-Exercises`

#### 2.2. SonarCloud Yapılandırmasını Doğrulama

**Kontrol Edilecekler:**
- `sonar-project.properties` dosyasındaki bilgiler doğru mu?
- Organization ve project key eşleşiyor mu?

**Gerekirse Düzenleme:**
```bash
# sonar-project.properties dosyasını aç ve doğrula
cat sonar-project.properties

# Eğer organization veya project key farklıysa düzenle
# Organization: SonarCloud'daki organization ismin
# Project Key: SonarCloud'da gösterilen project key
```

---

### 3. GitHub Actions İzinlerini Yapılandırma

#### 3.1. Workflow Permissions

**Neden Gerekli:** Container Registry'ye push yapabilmek için

**Adımlar:**
1. GitHub repository → **Settings** sekmesine git
2. **Actions** → **General** sekmesine tıkla
3. Aşağı kaydır ve **Workflow permissions** bölümüne gel
4. Şu seçenekleri seç:
   - ✅ **Read and write permissions**
   - ✅ **Allow GitHub Actions to create and approve pull requests**
5. **Save** butonuna bas

#### 3.2. GitHub Packages Permissions

**Adımlar:**
1. Settings → **Actions** → **General**
2. **Workflow permissions** bölümünde:
   - ✅ Read and write permissions seçili olmalı
3. Bu sayede GHCR'ye (GitHub Container Registry) push yapılabilir

---

### 4. Environment Yapılandırması (Opsiyonel ama Önerilen)

#### 4.1. NuGet Production Environment Oluşturma

**Neden Gerekli:** Yanlışlıkla NuGet'e paket yayınlamayı önlemek için

**Adımlar:**
1. Settings → **Environments** sekmesine git
2. **New environment** butonuna bas
3. Environment bilgilerini gir:
   - **Name:** `nuget-production`
4. **Configure environment** butonuna bas
5. Protection rules ekle:
   - ✅ **Required reviewers:** 1 (kendi kullanıcı adını ekle)
   - ⏰ **Wait timer:** 0 minutes (opsiyonel)
   - 🌿 **Deployment branches:** `main` ve `master` (diğerleri bloklu)
6. **Save protection rules** butonuna bas

#### 4.2. Staging Environment (İleride kullanmak için)

**Adımlar:**
1. **New environment** → Name: `staging`
2. Protection rules: Yok (otomatik deployment için)
3. **Environment URL:** `https://staging.example.com` (şimdilik placeholder)

#### 4.3. Production Environment (İleride kullanmak için)

**Adımlar:**
1. **New environment** → Name: `production`
2. Protection rules:
   - ✅ Required reviewers: 2
   - ⏰ Wait timer: 30 minutes
3. **Environment URL:** `https://example.com` (şimdilik placeholder)

---

## 🔄 Test ve Doğrulama

### 5. CI Pipeline'ı Test Etme

#### 5.1. Pull Request ile Test

**Adımlar:**
1. Yeni bir branch oluştur:
```bash
git checkout -b test/ci-pipeline
```

2. Küçük bir değişiklik yap:
```bash
echo "# CI/CD Test" >> docs/CI_TEST.md
git add docs/CI_TEST.md
git commit -m "test: verify CI pipeline"
git push origin test/ci-pipeline
```

3. GitHub'da Pull Request oluştur:
   - **Compare:** `test/ci-pipeline` → `main`
   - **Title:** `Test: Verify CI Pipeline`
   - **Create pull request** butonuna bas

4. Actions sekmesinde workflow'ların çalıştığını kontrol et:
   - ✅ **CI Pipeline** - Çalışıyor mu?
   - ✅ **Build & Test** - 3 platform (Ubuntu, Windows, macOS) çalışıyor mu?
   - ✅ **Code Quality** - SonarCloud analizi çalışıyor mu?
   - ✅ **Coverage Gate** - Coverage raporu oluşuyor mu?

#### 5.2. Beklenen Sonuçlar

**Başarılı Olması Gerekenler:**
- ✅ Build & Test (ubuntu-latest) - PASS
- ✅ Build & Test (windows-latest) - PASS
- ✅ Build & Test (macos-latest) - PASS
- ✅ Code Quality & Static Analysis - PASS
- ⚠️ SonarCloud analysis - İlk kez çalışırsa bazı uyarılar normal

**Başarısız Olabilecekler (Normal):**
- ⚠️ Code Formatting - Eğer kod formatlanmamışsa düzelt:
```bash
dotnet format AdvancedCsharpConcepts.sln
git add .
git commit -m "style: format code"
git push
```

- ⚠️ SonarCloud Quality Gate - İlk kez çalışırsa threshold'ları ayarla

---

### 6. Container Build Pipeline'ı Test Etme

#### 6.1. Manuel Trigger ile Test

**Adımlar:**
1. GitHub → **Actions** sekmesine git
2. Sol taraftan **Build & Push Container** workflow'unu seç
3. **Run workflow** butonuna bas
4. Branch seç: `main` veya `test/ci-pipeline`
5. **Run workflow** onaylama butonuna bas

#### 6.2. Build Process'i İzleme

**Kontrol Edilecekler:**
- ✅ Docker image build ediliyor mu?
- ✅ Trivy security scan çalışıyor mu?
- ✅ Grype scan çalışıyor mu?
- ✅ Container startup test geçiyor mu?
- ✅ Dive analysis çalışıyor mu?

#### 6.3. Image Push (Sadece main/master branch)

**Not:** Pull Request'lerde image push edilmez, sadece build edilir.

**Main branch'e push edildiğinde:**
- ✅ Image GitHub Container Registry'ye push edilir
- ✅ Image Cosign ile imzalanır
- ✅ Build provenance attestation eklenir

---

### 7. NuGet Publishing Pipeline'ı Test Etme (Opsiyonel)

**⚠️ UYARI:** Bu gerçek NuGet.org'a paket yayınlar! Test için önce private registry kullanmayı düşün.

#### 7.1. Test Tag Oluşturma

**Adımlar:**
1. Eğer test etmek istiyorsan prerelease tag kullan:
```bash
git tag v0.0.1-test
git push origin v0.0.1-test
```

2. GitHub → **Actions** → **Publish to NuGet** workflow'unu izle

3. Workflow çalışırken:
   - ✅ Validate Release job - Version kontrolü
   - ✅ Build & Test - Test suite çalışıyor mu?
   - ✅ Pack Packages - .nupkg dosyaları oluşuyor mu?
   - ⏸️ Publish to NuGet - Environment approval bekliyor

4. **Environment approval:**
   - Workflow sana bildirim gönderecek
   - **Review deployments** butonuna bas
   - **Approve and deploy** veya **Reject** seç

**İlk Test için Öneri:**
- ❌ Gerçek versiyonla (v1.0.0) test etme
- ✅ Test versiyonuyla (v0.0.1-test) dene
- ✅ Veya manual workflow dispatch kullan

---

## 📋 Kontrol Listesi

### Önce Yapılması Gerekenler (Kritik)

- [ ] **NUGET_API_KEY** secret'ı eklendi
- [ ] **SONAR_TOKEN** secret'ı eklendi
- [ ] GitHub Actions workflow permissions ayarlandı (Read & Write)
- [ ] SonarCloud projesi oluşturuldu ve yapılandırıldı
- [ ] `nuget-production` environment oluşturuldu (approval rules ile)

### Test Adımları

- [ ] Test branch oluşturuldu ve PR açıldı
- [ ] CI Pipeline PR'da çalıştı ve geçti
- [ ] Code formatting kontrol edildi (gerekirse `dotnet format` çalıştırıldı)
- [ ] SonarCloud analizi çalıştı
- [ ] Coverage report oluştu
- [ ] Container build workflow manuel çalıştırıldı
- [ ] Security scan'ler (Trivy, Grype) geçti
- [ ] Container startup test geçti

### Opsiyonel ama Önerilen

- [ ] Staging environment oluşturuldu
- [ ] Production environment oluşturuldu
- [ ] README.md'ye workflow badges eklendi
- [ ] NuGet publishing test edildi (prerelease tag ile)
- [ ] Container image pull edildi ve local'de test edildi

---

## 🚨 Sorun Giderme

### "SONAR_TOKEN not provided" Hatası

**Çözüm:**
1. SonarCloud'dan yeni token al
2. GitHub Secrets'a ekle
3. Workflow'u yeniden çalıştır

### "Permission denied" - Container Registry

**Çözüm:**
1. Settings → Actions → General → Workflow permissions
2. "Read and write permissions" seç
3. Save

### NuGet Push "409 Conflict" Hatası

**Çözüm:**
- Bu version zaten yayınlanmış
- Version numarasını artır (v1.0.1, v1.0.2, vb.)

### Code Formatting Fails

**Çözüm:**
```bash
# Tüm kodu formatla
dotnet format AdvancedCsharpConcepts.sln

# Değişiklikleri commit et
git add .
git commit -m "style: format code"
git push
```

---

## 📊 İlk Kurulum Sonrası Beklenen Durum

### GitHub Actions Tab

**Workflows görünür olmalı:**
- ✅ CI Pipeline
- ✅ Build & Push Container
- ✅ Publish to NuGet

### Secrets

**Settings → Secrets and variables → Actions:**
- ✅ NUGET_API_KEY
- ✅ SONAR_TOKEN
- ✅ GITHUB_TOKEN (otomatik)

### Environments

**Settings → Environments:**
- ✅ nuget-production (approval required)
- ✅ staging (opsiyonel)
- ✅ production (opsiyonel)

### SonarCloud

**Dashboard görünümü:**
- ✅ Proje import edilmiş
- ✅ İlk analiz tamamlanmış
- ✅ Quality Gate tanımlı (varsayılan veya özel)

---

## 🎉 Tamamlandıktan Sonra Kullanım

### NuGet Paketi Yayınlama

```bash
# Version tag oluştur
git tag v1.0.0

# Tag'i push et
git push origin v1.0.0

# GitHub Actions otomatik olarak:
# 1. Testleri çalıştırır
# 2. Paketleri build eder
# 3. NuGet'e yayınlar (approval sonrası)
# 4. GitHub Release oluşturur
```

### Container Image Build

```bash
# Sadece main'e push et
git push origin main

# GitHub Actions otomatik olarak:
# 1. Multi-arch image build eder
# 2. Security scan'ler yapar
# 3. GHCR'ye push eder
# 4. Image'ı imzalar
```

### Pull Request Açma

```bash
# Feature branch oluştur
git checkout -b feature/my-feature

# Değişiklik yap ve push et
git push origin feature/my-feature

# PR oluştur - CI otomatik çalışır:
# 1. Build & test (3 platform)
# 2. Code quality check
# 3. Coverage report
# 4. SonarCloud analysis
# 5. Security scan
```

---

## 📚 Ek Kaynaklar

**Dokümantasyon:**
- [CI/CD Workflows - Detaylı Döküman](./CICD_WORKFLOWS.md)
- [Quick Start Guide](./QUICK_START_CICD.md)
- [Architecture Decision Records](./decisions/README.md)

**Dış Bağlantılar:**
- [SonarCloud Dashboard](https://sonarcloud.io/dashboard?id=dogaaydinn_CSharp-Covariance-Polymorphism-Exercises)
- [NuGet.org Packages](https://www.nuget.org/profiles/dogaaydinn)
- [GitHub Container Registry](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/pkgs/container/csharp-covariance-polymorphism-exercises)

---

## ⏰ Tahmini Süre

| Görev | Süre | Zorunluluk |
|-------|------|------------|
| GitHub Secrets yapılandırma | 10 dakika | ✅ Kritik |
| SonarCloud kurulum | 15 dakika | ✅ Kritik |
| Actions permissions | 5 dakika | ✅ Kritik |
| Environment kurulum | 10 dakika | ⚠️ Önerilen |
| Test workflows | 20 dakika | ⚠️ Önerilen |
| **TOPLAM** | **~1 saat** | - |

---

## ✅ Son Kontrol

Herşey tamamlandıktan sonra bu checklist'i işaretle:

- [ ] Tüm secrets eklendi ve doğrulandı
- [ ] SonarCloud başarıyla çalıştı
- [ ] Test PR'ı oluşturuldu ve CI geçti
- [ ] Container build test edildi
- [ ] Environment protection rules aktif
- [ ] Dokümantasyon okundu
- [ ] İlk başarılı build tamamlandı

**🎯 Başarılı kurulum sonrası:**
- CI/CD pipeline'lar otomatik çalışacak
- Her PR'da kalite kontrolleri yapılacak
- Tag push'larında otomatik NuGet yayını olacak
- Main branch'e her push'da container build edilecek

---

**Son Güncelleme:** 2024-12-02

**Hazırlayan:** Claude Code (Advanced CI/CD Implementation)
