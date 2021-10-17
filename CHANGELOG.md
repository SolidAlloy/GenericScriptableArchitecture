## [1.2.1](https://github.com/SolidAlloy/GenericScriptableArchitecture/compare/1.2.0...1.2.1) (2021-10-17)


### Bug Fixes

* Fixed exceptions when using UnityEvents interface and when creating new generic unity objects ([167318c](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/167318c4ad54c9a11ee24f496937ecee58e39b9f))

# [1.2.0](https://github.com/SolidAlloy/GenericScriptableArchitecture/compare/1.1.9...1.2.0) (2021-10-13)


### Features

* Added an ability to traverse the hierarchy of types in the dropdown ([2c1b045](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/2c1b04508403107759e40d10c006b64000e888ee))

## [1.1.9](https://github.com/SolidAlloy/GenericScriptableArchitecture/compare/1.1.8...1.1.9) (2021-10-07)


### Bug Fixes

* Fixed error in TimelineInternals.dll when Timeline package is not installed ([8c73f40](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/8c73f402037648812d2a0d7e403472847ca19252))

## [1.1.8](https://github.com/SolidAlloy/GenericScriptableArchitecture/compare/1.1.7...1.1.8) (2021-09-30)


### Bug Fixes

* Started saving config changes to disk immediately after a change in generated assemblies ([f08cc24](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/f08cc24e25abfe28a8c77bdc04709781fcdf7b68))

## [1.1.7](https://github.com/SolidAlloy/GenericScriptableArchitecture/compare/1.1.6...1.1.7) (2021-09-29)


### Bug Fixes

* Fixed MissingReferenceException sometimes appearing on MacOS ([dc3b3de](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/dc3b3de4fec71023d10a211bfafd3e6d64b7f51e))

## [1.1.6](https://github.com/SolidAlloy/GenericScriptableArchitecture/compare/1.1.5...1.1.6) (2021-09-27)


### Bug Fixes

* Integrated fixes from SolidUtilities and GenericUnityObjects ([0741d83](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/0741d83e2a15d247e4db4e9ae43f7c9e9ec0ff09))

## [1.1.5](https://github.com/SolidAlloy/GenericScriptableArchitecture/compare/1.1.4...1.1.5) (2021-09-16)


### Bug Fixes

* Fixed an issue with missing generic MonoBehaviours after pulling changes from git ([e25cc42](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/e25cc4276d8a59f8cb8003c0f8413a6e6af938ba))

## [1.1.4](https://github.com/SolidAlloy/GenericScriptableArchitecture/compare/1.1.3...1.1.4) (2021-09-05)


### Bug Fixes

* Updated GenericUnityObjects to fix issues with using git ([acf79ff](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/acf79ff751cb938a39e797e05a8ed7780e7a83d7))

## [1.1.3](https://github.com/SolidAlloy/GenericScriptableArchitecture/compare/1.1.2...1.1.3) (2021-08-31)


### Bug Fixes

* Deleted accidentally added cache file ([62f9563](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/62f95635bd6a333f3cbb5e8e333dbb2891c423bf))
* Fixed bugs in the GenericUnityObjects dependency package ([ebfd9d2](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/ebfd9d2e6897b49335f68fa1658de67062837fe7))

## [1.1.2](https://github.com/SolidAlloy/GenericScriptableArchitecture/compare/1.1.1...1.1.2) (2021-08-30)


### Bug Fixes

* Fixed icon assets not being loaded when installing a package from git or openupm ([e62b8c4](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/e62b8c4c92de59a2bda4bd31fa0ecd993e200459))

## [1.1.1](https://github.com/SolidAlloy/GenericScriptableArchitecture/compare/1.1.0...1.1.1) (2021-08-29)


### Bug Fixes

* Fixed compilation error when using Timeline 1.4.8 ([a0c9768](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/a0c976802ad819991293c6cc181d514346de88b8))

# [1.1.0](https://github.com/SolidAlloy/GenericScriptableArchitecture/compare/1.0.1...1.1.0) (2021-08-28)


### Bug Fixes

* Added TimelineInternals sln and csproj files to git repo ([22c30db](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/22c30db68ebdbec768017d3651543599587de185))


### Features

* Fixed emitters for new Timeline package version and added a custom action menu for generating generic emitters ([07ed92c](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/07ed92c35c84d697bea0b6444e65cac7771e58a9))
* Started generating emitters with each new concrete ScriptableEvent ([0090890](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/0090890d8f3c31b8ca801c37684b38bcb7fe836d))
* Started using the new ApplyToChildren attribute on ScriptableEventEmitters ([235f36e](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/235f36e4f6dcfce9a5501b6bffa06ef1f8fd4768))

## [1.0.1](https://github.com/SolidAlloy/GenericScriptableArchitecture/compare/1.0.0...1.0.1) (2021-08-23)


### Bug Fixes

* Fixed reference to the Timeline assembly ([566fb24](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/566fb2456a1c478ca32c2f4755b0861a08e3d22b))

# 1.0.0 (2021-08-22)


### Bug Fixes

* Added missing releaserc.json ([27af6e5](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/27af6e542a9f0d49897b64fd32734e519a822038))
* Fixed OnEnable not running for Variable and VariableWithHistory in Editor with PlayMode options enabled ([1f20864](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/1f20864cb2253cf30c4de5ff2227cc9ec0c3a769))
* In Variables, when current values is changed through inspector, the previous value is sent through event ([33321e6](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/33321e6d84a69d730b28f8ebd024116bb0cc25b1))


### Features

* Added CI to release the package ([dc7c47a](https://github.com/SolidAlloy/GenericScriptableArchitecture/commit/dc7c47a048cea7b1656ca600bdf89fd256d07913))
