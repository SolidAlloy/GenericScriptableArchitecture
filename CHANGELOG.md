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
