#!/usr/bin/env python3
"""Generate Unity 6.3 LTS project files + .meta GUIDs for the mobile client."""

from __future__ import annotations

import hashlib
from pathlib import Path

ROOT = Path("/workspace/UnityClient")


def guid_for(rel: str) -> str:
    return hashlib.md5(rel.encode("utf-8")).hexdigest()


def write(rel: str, text: str) -> None:
    path = ROOT / rel
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text, encoding="utf-8")


def meta_script(rel: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid_for(rel)}
MonoImporter:
  externalObjects: {{}}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {{instanceID: 0}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def meta_asmdef(rel: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid_for(rel)}
AssemblyDefinitionImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def meta_folder(rel: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid_for(rel)}
folderAsset: yes
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def meta_scene(rel: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid_for(rel)}
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


BOOT_GUID = guid_for("Assets/Scenes/Boot.unity")


def main() -> None:
    folders = [
        "Assets",
        "Assets/Scenes",
        "Assets/Scripts",
        "Assets/Scripts/Client",
        "Assets/Editor",
        "Assets/StreamingAssets",
        "Assets/StreamingAssets/PcData",
    ]
    for f in folders:
        (ROOT / f).mkdir(parents=True, exist_ok=True)
        write(f + ".meta", meta_folder(f))

    for cs in (ROOT / "Assets").rglob("*.cs"):
        rel = cs.relative_to(ROOT).as_posix()
        write(rel + ".meta", meta_script(rel))
    for asm in (ROOT / "Assets").rglob("*.asmdef"):
        rel = asm.relative_to(ROOT).as_posix()
        write(rel + ".meta", meta_asmdef(rel))

    write(
        "Packages/manifest.json",
        """{
  "dependencies": {
    "com.unity.2d.sprite": "1.0.0",
    "com.unity.ide.rider": "3.0.28",
    "com.unity.ide.visualstudio": "2.0.22",
    "com.unity.ugui": "2.0.0",
    "com.unity.modules.androidjni": "1.0.0",
    "com.unity.modules.animation": "1.0.0",
    "com.unity.modules.audio": "1.0.0",
    "com.unity.modules.imageconversion": "1.0.0",
    "com.unity.modules.imgui": "1.0.0",
    "com.unity.modules.jsonserialize": "1.0.0",
    "com.unity.modules.particlesystem": "1.0.0",
    "com.unity.modules.physics2d": "1.0.0",
    "com.unity.modules.screencapture": "1.0.0",
    "com.unity.modules.ui": "1.0.0",
    "com.unity.modules.uielements": "1.0.0",
    "com.unity.modules.unitywebrequest": "1.0.0"
  }
}
""",
    )

    write("ProjectSettings/ProjectVersion.txt", "m_EditorVersion: 6000.3.22f1\nm_EditorVersionWithRevision: 6000.3.22f1 (1c726e1fb402)\n")

    write(
        "ProjectSettings/EditorBuildSettings.asset",
        f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!1045 &1
EditorBuildSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Scenes:
  - enabled: 1
    path: Assets/Scenes/Boot.unity
    guid: {BOOT_GUID}
  m_configObjects: {{}}
""",
    )

    write(
        "ProjectSettings/ProjectSettings.asset",
        """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!129 &1
PlayerSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 23
  productGUID: 9c68a017babb7c229b2ba6be3b789c68
  AndroidProfiler: 0
  AndroidFilterTouchesWhenObscured: 0
  AndroidEnableSustainedPerformanceMode: 0
  defaultScreenOrientation: 3
  targetDevice: 2
  useOnDemandResources: 0
  accelerometerFrequency: 60
  companyName: GunMobile
  productName: Gun Mobile
  defaultCursor: {fileID: 0}
  cursorHotspot: {x: 0, y: 0}
  m_SplashScreenBackgroundColor: {r: 0.05, g: 0.07, b: 0.1, a: 1}
  m_ShowUnitySplashScreen: 1
  m_ShowUnitySplashLogo: 1
  m_SplashScreenOverdraw: 1
  allowedAutorotateToPortrait: 0
  allowedAutorotateToPortraitUpsideDown: 0
  allowedAutorotateToLandscapeRight: 1
  allowedAutorotateToLandscapeLeft: 1
  useOSAutorotation: 1
  use32BitDisplayBuffer: 1
  preserveFramebufferAlpha: 0
  disableDepthAndStencilBuffers: 0
  androidStartInFullscreen: 1
  androidRenderOutsideSafeArea: 1
  androidUseSwappy: 1
  defaultIsNativeResolution: 1
  macRetinaSupport: 1
  runInBackground: 1
  captureSingleScreen: 0
  muteOtherAudioSources: 0
  Prepare IOS For Recording: 0
  Force IOS Speakers When Recording: 0
  audioSpatialExperience: 0
  usePlayerLog: 1
  bakeCollisionMeshes: 0
  forceSingleInstance: 0
  useFlipModelSwapchain: 1
  resizableWindow: 0
  useMacAppStoreValidation: 0
  macAppStoreCategory: public.app-category.games
  gpuSkinning: 0
  xboxPIXTextureCapture: 0
  xboxEnableAvatar: 0
  xboxEnableKinect: 0
  xboxEnableKinectAutoTracking: 0
  xboxEnableFitness: 0
  visibleInBackground: 1
  allowFullscreenSwitch: 1
  fullscreenMode: 1
  xboxSpeechDB: 0
  xboxEnableHeadOrientation: 0
  xboxEnableGuest: 0
  xboxEnablePIXSampling: 0
  metalFramebufferOnly: 0
  xboxOneDisableNativeBackbuffer: 0
  xboxOneEnableTypeOptimization: 0
  xboxOnePresentImmediateThreshold: 0
  switchQueueCommandMemory: 0
  switchQueueControlMemory: 16384
  switchQueueComputeMemory: 262144
  switchNVNShaderPoolsGranularity: 33554432
  switchNVNDefaultPoolsGranularity: 16777216
  switchNVNOtherPoolsGranularity: 16777216
  stadiaPresentMode: 0
  stadiaTargetFramerate: 0
  vulkanNumSwapchainBuffers: 3
  vulkanEnableSetSRGBWrite: 0
  vulkanEnablePreTransform: 0
  vulkanEnableLateAcquireNextImage: 0
  m_ColorGamuts: 00000000
  targetPixelDensity: 300
  resolutionScalingMode: 0
  androidSupportedAspectRatio: 1
  androidMaxAspectRatio: 2.4
  applicationIdentifier:
    Android: com.gunmobile.client
    iPhone: com.gunmobile.client
  buildNumber:
    Standalone: 0
    iPhone: 0
    tvOS: 0
  overrideDefaultApplicationIdentifier: 1
  AndroidBundleVersionCode: 1
  AndroidMinSdkVersion: 23
  AndroidTargetSdkVersion: 33
  AndroidPreferredInstallLocation: 1
  aotOptions: 
  stripEngineCode: 1
  iPhoneStrippingLevel: 0
  iPhoneScriptCallOptimization: 0
  ForceInternetPermission: 1
  ForceSDCardPermission: 0
  CreateWallpaper: 0
  APKExpansionFiles: 0
  keepLoadedShadersAlive: 0
  StripUnusedMeshComponents: 1
  VertexChannelCompressionMask: 4054
  iPhoneSdkVersion: 988
  iOSSimulatorArchitecture: 0
  iOSTargetOSVersionString: 12.0
  tvOSSdkVersion: 0
  tvOSSimulatorArchitecture: 0
  tvOSRequireExtendedGameController: 0
  tvOSTargetOSVersionString: 12.0
  uIPrerenderedIcon: 0
  uIRequiresPersistentWiFi: 0
  uIRequiresFullScreen: 1
  uIStatusBarHidden: 1
  uIExitOnSuspend: 0
  uIStatusBarStyle: 0
  appleTVSplashScreen: {fileID: 0}
  appleTVSplashScreenOverscan: {fileID: 0}
  xboxOneResolution: 0
  xboxOneSResolution: 0
  xboxOneXResolution: 3
  xboxOneMonoLoggingLevel: 0
  xboxOneLoggingLevel: 1
  xboxOneDisableEsram: 0
  xboxOneEnableTypeOptimization: 0
  xboxOnePresentImmediateThreshold: 0
  switchQueueCommandMemory: 1048576
  switchQueueControlMemory: 16384
  switchQueueComputeMemory: 262144
  switchNVNShaderPoolsGranularity: 33554432
  m_BuildTargetIcons: []
  m_BuildTargetPlatformIcons: []
  m_BuildTargetBatching: []
  m_BuildTargetGraphicsJobs: []
  m_BuildTargetGraphicsJobMode: []
  m_BuildTargetGraphicsAPIs: []
  m_BuildTargetVRSettings: []
  openGLRequireES31: 0
  openGLRequireES31AEP: 0
  openGLRequireES32: 0
  m_TemplateCustomTags: {}
  mobileMTRendering:
    Android: 1
    iPhone: 1
    tvOS: 1
  m_BuildTargetGroupLightmapEncodingQuality: []
  m_BuildTargetGroupLightmapSettings: []
  playModeTestRunnerEnabled: 0
  runPlayModeTestAsEditModeTest: 0
  actionOnDotNetUnhandledException: 1
  enableInternalProfiler: 0
  logObjCUncaughtExceptions: 1
  enableCrashReportAPI: 0
  cameraUsageDescription: 
  locationUsageDescription: 
  microphoneUsageDescription: 
  bluetoothUsageDescription: 
  switchNMETAOverlayFile: 
  switchLTOSetting: 0
  switchApplicationID: 0x01004b9000490000
  switchNSOEnable: 0
  switchNetLibKey: 
  switchSocketMemoryPoolSize: 6144
  switchSocketAllocatorPoolSize: 128
  switchSocketConcurrencyLimit: 14
  switchScreenResolutionBehavior: 2
  switchUseCPUProfiler: 0
  switchUseGOLDLinker: 0
  switchLTOSetting: 0
  switchApplicationErrorCodeCheck: 0
  n3dsUseExtSaveData: 0
  ps4PronunciationXMLPath: 
  ps4PronunciationSIGPath: 
  ps4BackgroundImagePath: 
  ps4StartupImagePath: 
  ps4StartupImagesFolder: 
  ps4IconImagesFolder: 
  ps4SaveDataImagePath: 
  ps4SdkOverride: 
  ps4BGMPath: 
  ps4ShareFilePath: 
  ps4ShareOverlayImagePath: 
  ps4PrivacyGuardImagePath: 
  ps4ExtraSceSysFile: 
  ps4NPtitleDatPath: 
  ps4RemotePlayKeyAssignment: -1
  ps4RemotePlayKeyMappingDir: 
  ps4PlayTogetherPlayerCount: 0
  ps4EnterButtonAssignment: 1
  ps4ApplicationParam1: 0
  ps4ApplicationParam2: 0
  ps4ApplicationParam3: 0
  ps4ApplicationParam4: 0
  ps4DownloadDataSize: 0
  ps4GarlicHeapSize: 2048
  ps4ProGarlicHeapSize: 2560
  playerPrefsMaxSize: 32768
  ps4Passcode: frAQBc8Wsa1xVPfvJcrgRYwTiizs2trQ
  ps4pnSessions: 1
  ps4pnPresence: 1
  ps4pnFriends: 1
  ps4pnGameCustomData: 1
  playerPrefsSupport: 0
  enableApplicationExit: 0
  resetTempFolder: 1
  restrictedAudioUsageRights: 0
  ps4UseResolutionFallback: 0
  ps4ReprojectionSupport: 0
  ps4UseAudio3dBackend: 0
  ps4UseLowGarlicFragmentationMode: 1
  ps4SocialScreenEnabled: 0
  ps4ScriptOptimizationLevel: 0
  ps4Audio3dVirtualSpeakerCount: 14
  ps4attribCpuUsage: 0
  ps4PatchPkgPath: 
  ps4PatchLatestPkgPath: 
  ps4PatchChangeinfoPath: 
  ps4PatchDayOne: 0
  ps4attribUserManagement: 0
  ps4attribMoveSupport: 0
  ps4attrib3DSupport: 0
  ps4attribShareSupport: 0
  ps4attribExclusiveVR: 0
  ps4disableAutoHideSplash: 0
  ps4videoRecordingFeaturesUsed: 0
  ps4contentSearchFeaturesUsed: 0
  ps4CompatibilityPS5: 0
  ps4GPU800MHz: 1
  ps4attribEyeToEyeDistanceSettingVR: 0
  ps4IncludedModules: []
  ps4attribVROutputEnabled: 0
  monoEnv: 
  splashScreenBackgroundSourceLandscape: {fileID: 0}
  splashScreenBackgroundSourcePortrait: {fileID: 0}
  blurSplashScreenBackground: 1
  spritePackerPolicy: 
  webGLMemorySize: 16
  webGLExceptionSupport: 1
  webGLNameFilesAsHashes: 0
  webGLDataCaching: 1
  webGLDebugSymbols: 0
  webGLEmscriptenArgs: 
  webGLModulesDirectory: 
  webGLTemplate: APPLICATION:Default
  webGLAnalyzeBuildSize: 0
  webGLUseEmbeddedResources: 0
  webGLCompressionFormat: 1
  webGLWasmArithmeticExceptions: 0
  webGLLinkerTarget: 1
  webGLThreadsSupport: 0
  webGLDecompressionFallback: 0
  scriptingDefineSymbols: {}
  additionalCompilerArguments: {}
  platformArchitecture: {}
  scriptingBackend:
    Android: 1
    iPhone: 1
  il2cppCompilerConfiguration: {}
  managedStrippingLevel: {}
  incrementalIl2cppBuild: {}
  suppressCommonWarnings: 1
  allowUnsafeCode: 0
  useDeterministicCompilation: 1
  enableRoslynAnalyzers: 1
  additionalIl2CppArgs: 
  scriptingRuntimeVersion: 1
  gcIncremental: 1
  assemblyVersionValidation: 1
  gcWBarrierValidation: 0
  apiCompatibilityLevelPerPlatform:
    Android: 3
    iPhone: 3
  m_RenderingPath: 1
  m_MobileRenderingPath: 1
  metroPackageName: GunMobile
  metroPackageVersion: 
  metroCertificatePath: 
  metroCertificatePassword: 
  metroCertificateSubject: 
  metroCertificateIssuer: 
  metroCertificateNotAfter: 0000000000000000
  metroApplicationDescription: Gun Mobile
  wsaImages: {}
  metroTileShortName: 
  metroTileShowName: 0
  metroMediumTileShowName: 0
  metroLargeTileShowName: 0
  metroWideTileShowName: 0
  metroSupportStreamingInstall: 0
  metroLastRequiredScene: 0
  metroDefaultTileSize: 1
  metroTileForegroundText: 2
  metroTileBackgroundColor: {r: 0.13333, g: 0.17254, b: 0.21568, a: 1}
  metroSplashScreenBackgroundColor: {r: 0.12941, g: 0.17254, b: 0.21568, a: 1}
  metroSplashScreenUseBackgroundColor: 0
  platformCapabilities: {}
  metroTargetDeviceFamilies: {}
  metroFTAName: 
  metroFTAFileTypes: []
  metroProtocolName: 
  XboxOneProductId: 
  XboxOneUpdateKey: 
  XboxOneSandboxId: 
  XboxOneContentId: 
  XboxOneTitleId: 
  XboxOneSCId: 
  XboxOneGameOsOverridePath: 
  XboxOnePackagingOverridePath: 
  XboxOneAppManifestOverridePath: 
  XboxOneVersion: 1.0.0.0
  XboxOnePackageEncryption: 0
  XboxOnePackageUpdateGranularity: 2
  XboxOneDescription: 
  XboxOneLanguage:
  - enus
  XboxOneCapability: []
  XboxOneGameRating: {}
  XboxOneIsContentPackage: 0
  XboxOneEnhancedXboxCompatibilityMode: 0
  XboxOneEnableGPUVariability: 1
  XboxOneSockets: {}
  XboxOneSplashScreen: {fileID: 0}
  XboxOneAllowedProductIds: []
  XboxOnePersistentLocalStorageSize: 0
  XboxOneXTitleMemory: 8
  XboxOneOverrideIdentityName: 
  XboxOneOverrideIdentityPublisher: 
  vrEditorSettings: {}
  cloudServicesEnabled: {}
  luminIsChannelApp: 0
  luminVersion:
    m_VersionCode: 1
    m_VersionName: 
  apiCompatibilityLevel: 6
  cloudProjectId: 
  framebufferDepthMemorylessMode: 0
  qualitySettingsNames: []
  projectName: 
  organizationId: 
  cloudEnabled: 0
  enableNativePlatformBackendsForNewInputSystem: 0
  disableOldInputManagerSupport: 0
  legacyClampBlendShapeWeights: 0
  virtualTexturingSupportEnabled: 0
""",
    )

    write(
        "ProjectSettings/TagManager.asset",
        """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!78 &1
TagManager:
  serializedVersion: 2
  tags: []
  layers:
  - Default
  - TransparentFX
  - Ignore Raycast
  - 
  - Water
  - UI
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  m_SortingLayers:
  - name: Default
    uniqueID: 0
    locked: 0
""",
    )

    write(
        "ProjectSettings/EditorSettings.asset",
        """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!159 &1
EditorSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 11
  m_SerializationMode: 2
  m_LineEndingsForNewScripts: 1
  m_DefaultBehaviorMode: 1
  m_PrefabRegularEnvironment: {fileID: 0}
  m_PrefabUIEnvironment: {fileID: 0}
  m_SpritePackerMode: 4
  m_SpritePackerPaddingPower: 1
  m_Bc7TextureCompressor: 0
  m_EtcTextureCompressorBehavior: 1
  m_EtcTextureFastCompressor: 1
  m_EtcTextureNormalCompressor: 2
  m_EtcTextureBestCompressor: 4
  m_ProjectGenerationIncludedExtensions: txt;xml;fnt;cd;asmdef;rsp;asmref
  m_ProjectGenerationRootNamespace: GunMobile
  m_EnableTextureStreamingInPlayMode: 1
  m_EnableTextureStreamingInEditMode: 0
  m_AsyncShaderCompilation: 1
  m_CachingShaderPreprocessor: 1
  m_PrefabModeAllowAutoSave: 1
  m_EnterPlayModeOptionsEnabled: 0
  m_EnterPlayModeOptions: 3
  m_GameObjectNamingDigits: 1
  m_GameObjectNamingScheme: 0
  m_AssetNamingUsesSpace: 1
  m_UseLegacyProbeSampleCount: 0
  m_SerializeInlineMappingsOnOneLine: 1
  m_DisableCookiesInLightmapper: 1
  m_AssetPipelineMode: 1
  m_CacheServerMode: 0
  m_CacheServerEndpoint: 
  m_CacheServerNamespacePrefix: default
  m_CacheServerEnableDownload: 1
  m_CacheServerEnableUpload: 1
  m_CacheServerEnableAuth: 0
  m_CacheServerEnableTls: 0
""",
    )

    write(
        "Assets/Scenes/Boot.unity",
        f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_OcclusionBakeSettings:
    smallestOccluder: 5
    smallestHole: 0.25
    backfaceThreshold: 100
  m_SceneGUID: 00000000000000000000000000000000
  m_OcclusionCullingData: {{fileID: 0}}
--- !u!104 &2
RenderSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 9
  m_Fog: 0
  m_FogColor: {{r: 0.5, g: 0.5, b: 0.5, a: 1}}
  m_FogMode: 3
  m_FogDensity: 0.01
  m_LinearFogStart: 0
  m_LinearFogEnd: 300
  m_AmbientSkyColor: {{r: 0.2, g: 0.2, b: 0.2, a: 1}}
  m_AmbientEquatorColor: {{r: 0.2, g: 0.2, b: 0.2, a: 1}}
  m_AmbientGroundColor: {{r: 0.047, g: 0.043, b: 0.035, a: 1}}
  m_AmbientIntensity: 1
  m_AmbientMode: 3
  m_SubtractiveShadowColor: {{r: 0.42, g: 0.478, b: 0.627, a: 1}}
  m_SkyboxMaterial: {{fileID: 0}}
  m_HaloStrength: 0.5
  m_FlareStrength: 1
  m_FlareFadeSpeed: 3
  m_HaloTexture: {{fileID: 0}}
  m_SpotCookie: {{fileID: 0}}
  m_DefaultReflectionMode: 0
  m_DefaultReflectionResolution: 128
  m_ReflectionBounces: 1
  m_ReflectionIntensity: 1
  m_CustomReflection: {{fileID: 0}}
  m_Sun: {{fileID: 0}}
  m_IndirectSpecularColor: {{r: 0, g: 0, b: 0, a: 1}}
  m_UseRadianceAmbientProbe: 0
--- !u!157 &3
LightmapSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 12
  m_GIWorkflowMode: 1
  m_GISettings:
    serializedVersion: 2
    m_BounceScale: 1
    m_IndirectOutputScale: 1
    m_AlbedoBoost: 1
    m_EnvironmentLightingMode: 0
    m_EnableBakedLightmaps: 0
    m_EnableRealtimeLightmaps: 0
  m_LightmapEditorSettings:
    serializedVersion: 12
    m_Resolution: 2
    m_BakeResolution: 40
    m_AtlasSize: 1024
    m_AO: 0
    m_AOMaxDistance: 1
    m_CompAOExponent: 1
    m_CompAOExponentDirect: 0
    m_ExtractAmbientOcclusion: 0
    m_Padding: 2
    m_LightmapParameters: {{fileID: 0}}
    m_LightmapsBakeMode: 1
    m_TextureCompression: 1
    m_FinalGather: 0
    m_FinalGatherFiltering: 1
    m_FinalGatherRayCount: 256
    m_ReflectionCompression: 2
    m_MixedBakeMode: 2
    m_BakeBackend: 1
    m_PVRSampling: 1
    m_PVRDirectSampleCount: 32
    m_PVRSampleCount: 512
    m_PVRBounces: 2
    m_PVREnvironmentSampleCount: 256
    m_PVREnvironmentReferencePointCount: 2048
    m_PVRFilteringMode: 1
    m_PVRDenoiserTypeDirect: 1
    m_PVRDenoiserTypeIndirect: 1
    m_PVRDenoiserTypeAO: 1
    m_PVRFilterTypeDirect: 0
    m_PVRFilterTypeIndirect: 0
    m_PVRFilterTypeAO: 0
    m_PVREnvironmentMIS: 1
    m_PVRCulling: 1
    m_PVRFilteringGaussRadiusDirect: 1
    m_PVRFilteringGaussRadiusIndirect: 5
    m_PVRFilteringGaussRadiusAO: 2
    m_PVRFilteringAtrousPositionSigmaDirect: 0.5
    m_PVRFilteringAtrousPositionSigmaIndirect: 2
    m_PVRFilteringAtrousPositionSigmaAO: 1
    m_ExportTrainingData: 0
    m_TrainingDataDestination: TrainingData
    m_LightProbeSampleCountMultiplier: 4
  m_LightingDataAsset: {{fileID: 0}}
  m_LightingSettings: {{fileID: 0}}
--- !u!196 &4
NavMeshSettings:
  serializedVersion: 2
  m_ObjectHideFlags: 0
  m_BuildSettings:
    serializedVersion: 2
    agentTypeID: 0
    agentRadius: 0.5
    agentHeight: 2
    agentSlope: 45
    agentClimb: 0.4
    ledgeDropHeight: 0
    maxJumpAcrossDistance: 0
    minRegionArea: 2
    manualCellSize: 0
    cellSize: 0.16666667
    manualTileSize: 0
    tileSize: 256
    accuratePlacement: 0
    maxJobWorkers: 0
    preserveTilesOutsideBounds: 0
    debug:
      m_Flags: 0
  m_NavMeshData: {{fileID: 0}}
--- !u!1 &1000
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 1001}}
  - component: {{fileID: 1002}}
  m_Layer: 0
  m_Name: Main Camera
  m_TagString: MainCamera
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &1001
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 1000}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: -10}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_Children: []
  m_Father: {{fileID: 0}}
  m_RootOrder: 0
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
--- !u!20 &1002
Camera:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 1000}}
  m_Enabled: 1
  serializedVersion: 2
  m_ClearFlags: 2
  m_BackGroundColor: {{r: 0.05, g: 0.07, b: 0.1, a: 1}}
  m_projectionMatrixMode: 1
  m_GateFitMode: 2
  m_FOVAxisMode: 0
  m_SensorSize: {{x: 36, y: 24}}
  m_LensShift: {{x: 0, y: 0}}
  m_FocalLength: 50
  m_NormalizedViewPortRect:
    serializedVersion: 2
    x: 0
    y: 0
    width: 1
    height: 1
  near clip plane: -10
  far clip plane: 100
  field of view: 60
  orthographic: 1
  orthographic size: 5
  m_Depth: -1
  m_CullingMask:
    serializedVersion: 2
    m_Bits: 4294967295
  m_RenderingPath: -1
  m_TargetTexture: {{fileID: 0}}
  m_TargetDisplay: 0
  m_TargetEye: 3
  m_HDR: 0
  m_AllowMSAA: 0
  m_AllowDynamicResolution: 0
  m_ForceIntoRT: 0
  m_OcclusionCulling: 0
  m_StereoConvergence: 10
  m_StereoSeparation: 0.022
""",
    )
    write("Assets/Scenes/Boot.unity.meta", meta_scene("Assets/Scenes/Boot.unity"))
    print("project files written, boot guid", BOOT_GUID)


if __name__ == "__main__":
    main()
